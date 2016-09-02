namespace GongSolutions.Wpf.DragDrop.Utilities
{
  using System.Diagnostics.CodeAnalysis;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Media;

  /// <summary>
  /// A helper class for Dpi logicm cause Microsoft hides this with the internal flag.
  /// </summary>
  public static class DpiHelper
  {
    private static Matrix _transformToDevice;
    private static Matrix _transformToLogical;

    public static double DpiX = 0d;
    public static double DpiY = 0d;

    [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
    static DpiHelper()
    {
      var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", BindingFlags.NonPublic | BindingFlags.Static);
      var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", BindingFlags.NonPublic | BindingFlags.Static);

      var pixelsPerInchX = (int) dpiXProperty.GetValue(null, null); //SystemParameters.DpiX;
      DpiX = (double) pixelsPerInchX;
      var pixelsPerInchY = (int) dpiYProperty.GetValue(null, null); //SystemParameters.Dpi;
      DpiY = (double) pixelsPerInchY;

      _transformToLogical = Matrix.Identity;
      _transformToLogical.Scale(96d/(double) pixelsPerInchX, 96d/(double) pixelsPerInchY);
      _transformToDevice = Matrix.Identity;
      _transformToDevice.Scale((double) pixelsPerInchX/96d, (double) pixelsPerInchY/96d);
    }

    /// <summary>
    /// Convert a point in device independent pixels (1/96") to a point in the system coordinates.
    /// </summary>
    /// <param name="logicalPoint">A point in the logical coordinate system.</param>
    /// <returns>Returns the point converted to the system's coordinates.</returns>
    public static Point LogicalPixelsToDevice(Point logicalPoint)
    {
      return _transformToDevice.Transform(logicalPoint);
    }

    /// <summary>
    /// Convert a point in system coordinates to a point in device independent pixels (1/96").
    /// </summary>
    /// <param name="devicePoint">A point in the physical coordinate system.</param>
    /// <returns>Returns the point converted to the device independent coordinate system.</returns>
    public static Point DevicePixelsToLogical(Point devicePoint)
    {
      return _transformToLogical.Transform(devicePoint);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static Rect LogicalRectToDevice(Rect logicalRectangle)
    {
      Point topLeft = LogicalPixelsToDevice(new Point(logicalRectangle.Left, logicalRectangle.Top));
      Point bottomRight = LogicalPixelsToDevice(new Point(logicalRectangle.Right, logicalRectangle.Bottom));

      return new Rect(topLeft, bottomRight);
    }

    public static Rect DeviceRectToLogical(Rect deviceRectangle)
    {
      Point topLeft = DevicePixelsToLogical(new Point(deviceRectangle.Left, deviceRectangle.Top));
      Point bottomRight = DevicePixelsToLogical(new Point(deviceRectangle.Right, deviceRectangle.Bottom));

      return new Rect(topLeft, bottomRight);
    }

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    public static Size LogicalSizeToDevice(Size logicalSize)
    {
      Point pt = LogicalPixelsToDevice(new Point(logicalSize.Width, logicalSize.Height));

      return new Size {Width = pt.X, Height = pt.Y};
    }

    public static Size DeviceSizeToLogical(Size deviceSize)
    {
      Point pt = DevicePixelsToLogical(new Point(deviceSize.Width, deviceSize.Height));

      return new Size(pt.X, pt.Y);
    }

    public static Thickness LogicalThicknessToDevice(Thickness logicalThickness)
    {
      Point topLeft = LogicalPixelsToDevice(new Point(logicalThickness.Left, logicalThickness.Top));
      Point bottomRight = LogicalPixelsToDevice(new Point(logicalThickness.Right, logicalThickness.Bottom));

      return new Thickness(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
    }
  }
}