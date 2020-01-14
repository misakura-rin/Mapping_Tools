﻿using Mapping_Tools.Classes.MathUtil;
using Mapping_Tools.Classes.Tools;
using System.Collections.Generic;
using System.Linq;

namespace Mapping_Tools.Classes.SliderPathStuff {
    public class SliderPathUtil {
        public static List<Vector2> MoveAnchorsToLength(List<Vector2> anchors, PathType pathType, double newLength, out PathType newPathType) {
            var newAnchors = new List<Vector2>();
            var sliderPath = new SliderPath(pathType, anchors.ToArray(), newLength);
            var maxSliderPath = new SliderPath(pathType, anchors.ToArray());

            if (newLength > maxSliderPath.Distance) {
                // Extend linearly
                switch (pathType) {
                    case PathType.Bezier:
                        newPathType = PathType.Bezier;
                        newAnchors.AddRange(anchors);
                        newAnchors.Add(anchors.Last());
                        newAnchors.Add(sliderPath.PositionAt(1));
                        break;
                    case PathType.Catmull:
                    case PathType.PerfectCurve:
                        // Convert to bezier and then extend
                        newPathType = PathType.Bezier;
                        newAnchors = BezierConverter.ConvertToBezier(sliderPath).ControlPoints;
                        newAnchors.Add(anchors.Last());
                        newAnchors.Add(sliderPath.PositionAt(1));
                        break;
                    default:
                        newPathType = pathType;
                        newAnchors.AddRange(anchors);
                        newAnchors[newAnchors.Count - 1] = sliderPath.PositionAt(1);
                        break;
                }
            } else {
                switch (sliderPath.Type) {
                    case PathType.Catmull:
                    case PathType.Bezier:
                        newPathType = PathType.Bezier;

                        // Convert in case the path type is catmull
                        var convert = BezierConverter.ConvertToBezier(sliderPath).ControlPoints;

                        // Find the last bezier segment and the pixel length at that part
                        BezierSubdivision subdivision = null;
                        double totalLength = 0;

                        foreach (var bezierSubdivision in ChopAnchors(convert)) {
                            subdivision = bezierSubdivision;
                            var length = bezierSubdivision.SubdividedApproximationLength();

                            if (totalLength + length > newLength) {
                                break;
                            }

                            totalLength += length;
                            newAnchors.AddRange(bezierSubdivision.Points);
                        }

                        if (subdivision == null) break;

                        // Find T for the remaining pixel length
                        var t = subdivision.LengthToT(newLength - totalLength);

                        // ScaleRight the BezierSubdivision so the anchors end at T
                        subdivision.ScaleRight(t);

                        // Add the scaled anchors
                        newAnchors.AddRange(subdivision.Points);
                        break;
                    case PathType.PerfectCurve:
                        newPathType = PathType.PerfectCurve;
                        newAnchors.AddRange(anchors);
                        newAnchors[1] = sliderPath.PositionAt(0.5);
                        newAnchors[2] = sliderPath.PositionAt(1);
                        break;
                    default:
                        newPathType = pathType;
                        if (anchors.Count > 2) {
                            // Find the section of the linear slider which contains the slider end
                            totalLength = 0;
                            foreach (var bezierSubdivision in ChopAnchorsLinear(anchors)) {
                                newAnchors.Add(bezierSubdivision.Points[0]);
                                var length = bezierSubdivision.Length();

                                if (totalLength + length > newLength) {
                                    break;
                                }

                                totalLength += length;
                            }
                            newAnchors.Add(sliderPath.PositionAt(1));
                        } else {
                            newAnchors.AddRange(anchors);
                            newAnchors[newAnchors.Count - 1] = sliderPath.PositionAt(1);
                        }
                        break;
                }
            }

            return newAnchors;
        }

        public static IEnumerable<BezierSubdivision> ChopAnchors(List<Vector2> anchors) {
            int start = 0;
            int end = 0;

            for (int i = 0; i < anchors.Count; i++) {
                end++;

                if (i != anchors.Count - 1 && anchors[i] != anchors[i + 1]) continue;

                var cpSpan = anchors.GetRange(start, end - start);
                var subdivision = new BezierSubdivision(cpSpan);

                yield return subdivision;
                
                start = end;
            }
        }

        public static IEnumerable<BezierSubdivision> ChopAnchorsLinear(List<Vector2> anchors) {
            for (int i = 1; i < anchors.Count; i++) {
                var subdivision = new BezierSubdivision(new List<Vector2> {anchors[i - 1], anchors[i]});
                yield return subdivision;
            }
        }
    }
}