namespace FastReport.Export.Svg
{
    /// <summary>
    /// Specifies the alignment methods
    /// </summary>
    public enum Align
    {
        /// <summary>
        /// Do not force uniform scaling. Scale the graphic content of the given element non-uniformly
        /// if necessary such that the element's bounding box exactly matches the viewport rectangle.
        /// </summary>
        none,
        /// <summary>
        /// Force uniform scaling. Align the min-x of the element's viewBox with the smallest X value
        /// of the viewport. Align the min-y of the element's viewBox with the smallest Y value of the viewport.
        /// </summary>
        xMinYMin,
        /// <summary>
        /// Force uniform scaling. Align the midpoint X value of the element's viewBox with the midpoint
        /// X value of the viewport. Align the min-y of the element's viewBox with the smallest Y value
        /// of the viewport.
        /// </summary>
        xMidYMin,
        /// <summary>
        /// Force uniform scaling. Align the min-x+width of the element's viewBox with the maximum X value
        /// of the viewport. Align the min-y of the element's viewBox with the smallest Y value of the viewport.
        /// </summary>
        xMaxYMin,
        /// <summary>
        /// Force uniform scaling. Align the min-x of the element's viewBox with the smallest X value of
        /// the viewport. Align the midpoint Y value of the element's viewBox with the midpoint Y value 
        /// of the viewport.
        /// </summary>
        xMinYMid,
        /// <summary>
        /// The default. Force uniform scaling. Align the midpoint X value of the element's viewBox 
        /// with the midpoint X value of the viewport. Align the midpoint Y value of the element's 
        /// viewBox with the midpoint Y value of the viewport.
        /// </summary>
        xMidYMid,
        /// <summary>
        /// Force uniform scaling. Align the min-x+width of the element's viewBox with the maximum X
        /// value of the viewport. Align the midpoint Y value of the element's viewBox with the midpoint 
        /// Y value of the viewport.
        /// </summary>
        xMaxYMid,
        /// <summary>
        /// Force uniform scaling. Align the min-x of the element's viewBox with the smallest X value of 
        /// the viewport. Align the min-y+height of the element's viewBox with the maximum Y value of the viewport.
        /// </summary>
        xMinYMax,
        /// <summary>
        /// Force uniform scaling. Align the midpoint X value of the element's viewBox with the midpoint X
        /// value of the viewport. Align the min-y+height of the element's viewBox with the maximum Y value
        /// of the viewport.
        /// </summary>
        xMidYMax,
        /// <summary>
        /// Force uniform scaling. Align the min-x+width of the element's viewBox with the maximum X value of
        /// the viewport. Align the min-y+height of the element's viewBox with the maximum Y value of the viewport.
        /// </summary>
        xMaxYMax
    }

    /// <summary>
    /// Specifies the svg scale types
    /// </summary>
    public enum MeetOrSlice
    {
        /// <summary>
        ///  (the default) - Scale the graphic such that:
        ///  - aspect ratio is preserved
        ///  - the entire viewBox is visible within the viewport
        ///  - the viewBox is scaled up as much as possible, while still meeting the other criteria
        /// </summary>
        meet,
        /// <summary>
        /// Scale the graphic such that:
        /// - aspect ratio is preserved
        /// - the entire viewport is covered by the viewBox
        /// - the viewBox is scaled down as much as possible, while still meeting the other criteria
        /// </summary>
        slice
    }

    /// <summary>
    /// Describes scaling of a svg documents
    /// </summary>
    public class AspectRatio
    {
        private Align align;
        private MeetOrSlice? meetOrSlice;

        /// <summary>
        /// Gets the align value
        /// </summary>
        public Align Align { get { return align; } }

        /// <summary>
        /// Gets the meetOrSlice value
        /// </summary>
        public MeetOrSlice? MeetOrSlice { get { return meetOrSlice; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="AspectRatio"/> class.
        /// </summary>
        /// <param name="align"></param>
        public AspectRatio(Align align)
        {
            this.align = align;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="AspectRatio"/> class.
        /// </summary>
        /// <param name="align">Align value</param>
        /// <param name="meetOrSlice">meetOrSlice value</param>
        public AspectRatio(Align align, MeetOrSlice meetOrSlice) : this(align)
        {
            this.meetOrSlice = meetOrSlice;
        }
    }
}
