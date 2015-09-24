//----------------------------------------------
// ArrowLineBase.cs (c) 2007 by Charles Petzold
//----------------------------------------------

using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MIPSCoreUI.BaseObjects
{
    /// <summary>
    ///     Provides a base class for ArrowLine and ArrowPolyline.
    ///     This class is abstract.
    /// </summary>
    public abstract class ArrowLineBase : Shape
    {
        protected PathGeometry Pathgeo;
        protected PathFigure PathfigLine;
        protected PolyLineSegment PolysegLine;

        private readonly PathFigure pathfigHead1;
        private readonly PathFigure pathfigHead2;

        /// <summary>
        ///     Identifies the ArrowAngle dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowAngleProperty =
            DependencyProperty.Register("ArrowAngle",
                typeof(double), typeof(ArrowLineBase),
                new FrameworkPropertyMetadata(45.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the angle between the two sides of the arrowhead.
        /// </summary>
        public double ArrowAngle
        {
            set { SetValue(ArrowAngleProperty, value); }
            get { return (double)GetValue(ArrowAngleProperty); }
        }

        /// <summary>
        ///     Identifies the ArrowLength dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowLengthProperty =
            DependencyProperty.Register("ArrowLength",
                typeof(double), typeof(ArrowLineBase),
                new FrameworkPropertyMetadata(12.0,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the length of the two sides of the arrowhead.
        /// </summary>
        public double ArrowLength
        {
            set { SetValue(ArrowLengthProperty, value); }
            get { return (double)GetValue(ArrowLengthProperty); }
        }

        /// <summary>
        ///     Identifies the ArrowEnds dependency property.
        /// </summary>
        public static readonly DependencyProperty ArrowEndsProperty =
            DependencyProperty.Register("ArrowEnds",
                typeof(ArrowEnds), typeof(ArrowLineBase),
                new FrameworkPropertyMetadata(ArrowEnds.End,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the property that determines which ends of the
        ///     line have arrows.
        /// </summary>
        public ArrowEnds ArrowEnds
        {
            set { SetValue(ArrowEndsProperty, value); }
            get { return (ArrowEnds)GetValue(ArrowEndsProperty); }
        }

        /// <summary>
        ///     Identifies the CircleStart dependency property.
        /// </summary>
        public static readonly DependencyProperty CircleStartProperty =
            DependencyProperty.Register("CircleStart",
                typeof(bool), typeof(ArrowLineBase),
                new FrameworkPropertyMetadata(false,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the property that determines if the arrow start
        ///     is a circle.
        /// </summary>
        public bool CircleStart
        {
            set { SetValue(CircleStartProperty, value); }
            get { return (bool)GetValue(CircleStartProperty); }
        }

        /// <summary>
        ///     Identifies the IsArrowClosed dependency property.
        /// </summary>
        public static readonly DependencyProperty IsArrowClosedProperty =
            DependencyProperty.Register("IsArrowClosed",
                typeof(bool), typeof(ArrowLineBase),
                new FrameworkPropertyMetadata(false,
                        FrameworkPropertyMetadataOptions.AffectsMeasure));

        /// <summary>
        ///     Gets or sets the property that determines if the arrow head
        ///     is closed to resemble a triangle.
        /// </summary>
        public bool IsArrowClosed
        {
            set { SetValue(IsArrowClosedProperty, value); }
            get { return (bool)GetValue(IsArrowClosedProperty); }
        }

        /// <summary>
        ///     Initializes a new instance of ArrowLineBase.
        /// </summary>
        protected ArrowLineBase()
        {
            Pathgeo = new PathGeometry();

            PathfigLine = new PathFigure();
            PolysegLine = new PolyLineSegment();
            PathfigLine.Segments.Add(PolysegLine);

            pathfigHead1 = new PathFigure();
            var polysegHead1 = new PolyLineSegment();
            pathfigHead1.Segments.Add(polysegHead1);

            pathfigHead2 = new PathFigure();
            var polysegHead2 = new PolyLineSegment();
            pathfigHead2.Segments.Add(polysegHead2);
        }

        /// <summary>
        ///     Gets a value that represents the Geometry of the ArrowLine.
        /// </summary>
        protected override Geometry DefiningGeometry
        {
            get
            {
                var count = PolysegLine.Points.Count;

                if (count > 0)
                {
                    // Draw the arrow at the start of the line.
                    if ((ArrowEnds & ArrowEnds.Start) == ArrowEnds.Start)
                    {
                        var pt1 = PathfigLine.StartPoint;
                        var pt2 = PolysegLine.Points[0];
                        Pathgeo.Figures.Add(CalculateArrow(pathfigHead1, pt2, pt1));
                    }

                    // Draw the arrow at the end of the line.
                    if ((ArrowEnds & ArrowEnds.End) == ArrowEnds.End)
                    {
                        var pt1 = count == 1 ? PathfigLine.StartPoint :
                                                 PolysegLine.Points[count - 2];
                        var pt2 = PolysegLine.Points[count - 1];
                        Pathgeo.Figures.Add(CalculateArrow(pathfigHead2, pt1, pt2));
                    }

                    if (CircleStart)
                    {
                        var pt1 = PathfigLine.StartPoint;
                       
                        var ellipse1 = new EllipseGeometry(pt1, 1, 1);
                        var ellipse2 = new EllipseGeometry(pt1, 2, 2);
                        
                        Pathgeo.AddGeometry(ellipse1);
                        Pathgeo.AddGeometry(ellipse2);
                    }
                }
                return Pathgeo;
            }
        }
       

        PathFigure CalculateArrow(PathFigure pathfig, Point pt1, Point pt2)
        {
            var matx = new Matrix();
            var vect = pt1 - pt2;
            vect.Normalize();
            vect *= ArrowLength;

            var polyseg = pathfig.Segments[0] as PolyLineSegment;
            if (polyseg != null)
            {
                polyseg.Points.Clear();
                matx.Rotate(ArrowAngle / 2);
                pathfig.StartPoint = pt2 + vect * matx;
                polyseg.Points.Add(pt2);

                matx.Rotate(-ArrowAngle);
                polyseg.Points.Add(pt2 + vect * matx);
            }
            pathfig.IsClosed = IsArrowClosed;

            return pathfig;
        }
    }
}
