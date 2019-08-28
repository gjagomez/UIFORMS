Imports System.Drawing.Drawing2D
Imports System.ComponentModel
Imports System.Threading
Imports System.Drawing.Text
Imports System.Windows
Imports System.Runtime.InteropServices

Public Module Helpers

    Public Enum MouseState As Byte
        None = 0
        Over = 1
        Down = 2
    End Enum

    Public Enum RoundingStyle As Byte
        All = 0
        Top = 1
        Bottom = 2
        Left = 3
        Right = 4
        TopRight = 5
        BottomRight = 6
    End Enum

    Public Sub CenterString(G As Graphics, T As String, F As Font, C As Color, R As Rectangle)
        Dim TS As SizeF = G.MeasureString(T, F)

        Using B As New SolidBrush(C)
            G.DrawString(T, F, B, New Point(R.X + R.Width / 2 - (TS.Width / 2), R.Y + R.Height / 2 - (TS.Height / 2)))
        End Using
    End Sub

    Public Function ColorFromHex(Hex As String) As Color
        Return Color.FromArgb(Long.Parse(String.Format("FFFFFFFFFF{0}", Hex.Substring(1)), Globalization.NumberStyles.HexNumber))
    End Function

    Public Function FullRectangle(S As Size, Subtract As Boolean) As Rectangle

        If Subtract Then
            Return New Rectangle(0, 0, S.Width - 1, S.Height - 1)
        Else
            Return New Rectangle(0, 0, S.Width, S.Height)
        End If

    End Function

    Public Function RoundRect(ByVal Rect As Rectangle, ByVal Rounding As Integer, Optional ByVal Style As RoundingStyle = RoundingStyle.All) As GraphicsPath

        Dim GP As New GraphicsPath()
        Dim AW As Integer = Rounding * 2

        GP.StartFigure()

        If Rounding = 0 Then
            GP.AddRectangle(Rect)
            GP.CloseAllFigures()
            Return GP
        End If

        Select Case Style
            Case RoundingStyle.All
                GP.AddArc(New Rectangle(Rect.X, Rect.Y, AW, AW), -180, 90)
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90)
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90)
                GP.AddArc(New Rectangle(Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 90, 90)
            Case RoundingStyle.Top
                GP.AddArc(New Rectangle(Rect.X, Rect.Y, AW, AW), -180, 90)
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90)
                GP.AddLine(New Point(Rect.X + Rect.Width, Rect.Y + Rect.Height), New Point(Rect.X, Rect.Y + Rect.Height))
            Case RoundingStyle.Bottom
                GP.AddLine(New Point(Rect.X, Rect.Y), New Point(Rect.X + Rect.Width, Rect.Y))
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90)
                GP.AddArc(New Rectangle(Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 90, 90)
            Case RoundingStyle.Left
                GP.AddArc(New Rectangle(Rect.X, Rect.Y, AW, AW), -180, 90)
                GP.AddLine(New Point(Rect.X + Rect.Width, Rect.Y), New Point(Rect.X + Rect.Width, Rect.Y + Rect.Height))
                GP.AddArc(New Rectangle(Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 90, 90)
            Case RoundingStyle.Right
                GP.AddLine(New Point(Rect.X, Rect.Y + Rect.Height), New Point(Rect.X, Rect.Y))
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90)
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90)
            Case RoundingStyle.TopRight
                GP.AddLine(New Point(Rect.X, Rect.Y + 1), New Point(Rect.X, Rect.Y))
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Y, AW, AW), -90, 90)
                GP.AddLine(New Point(Rect.X + Rect.Width, Rect.Y + Rect.Height - 1), New Point(Rect.X + Rect.Width, Rect.Y + Rect.Height))
                GP.AddLine(New Point(Rect.X + 1, Rect.Y + Rect.Height), New Point(Rect.X, Rect.Y + Rect.Height))
            Case RoundingStyle.BottomRight
                GP.AddLine(New Point(Rect.X, Rect.Y + 1), New Point(Rect.X, Rect.Y))
                GP.AddLine(New Point(Rect.X + Rect.Width - 1, Rect.Y), New Point(Rect.X + Rect.Width, Rect.Y))
                GP.AddArc(New Rectangle(Rect.Width - AW + Rect.X, Rect.Height - AW + Rect.Y, AW, AW), 0, 90)
                GP.AddLine(New Point(Rect.X + 1, Rect.Y + Rect.Height), New Point(Rect.X, Rect.Y + Rect.Height))
        End Select

        GP.CloseAllFigures()

        Return GP

    End Function

End Module

#Region "UIButton"
Public Class UIButton
    Inherits Control

#Region "CreateRound"
    Private CreateRoundPath As GraphicsPath
    Private CreateRoundRectangle As Rectangle

    Function CreateRound(ByVal x As Integer, ByVal y As Integer, ByVal width As Integer, ByVal height As Integer, ByVal slope As Integer) As GraphicsPath
        CreateRoundRectangle = New Rectangle(x, y, width, height)
        Return CreateRound(CreateRoundRectangle, slope)
    End Function

    Function CreateRound(ByVal r As Rectangle, ByVal slope As Integer) As GraphicsPath
        CreateRoundPath = New GraphicsPath(FillMode.Winding)
        CreateRoundPath.AddArc(r.X, r.Y, slope, slope, 180.0F, 90.0F)
        CreateRoundPath.AddArc(r.Right - slope, r.Y, slope, slope, 270.0F, 90.0F)
        CreateRoundPath.AddArc(r.Right - slope, r.Bottom - slope, slope, slope, 0.0F, 90.0F)
        CreateRoundPath.AddArc(r.X, r.Bottom - slope, slope, slope, 90.0F, 90.0F)
        CreateRoundPath.CloseFigure()
        Return CreateRoundPath
    End Function
#End Region

#Region "Mouse states"
    Private State As MouseStates
    Enum MouseStates
        None = 0
        Over = 1
        Down = 2
    End Enum

    Protected Overrides Sub OnMouseEnter(ByVal e As EventArgs)
        State = MouseStates.Over
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseLeave(ByVal e As EventArgs)
        State = MouseStates.None
        Invalidate()
        MyBase.OnMouseEnter(e)
    End Sub

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        State = MouseStates.Down
        Invalidate()
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        State = MouseStates.Over
        Invalidate()

        If e.Button = Forms.MouseButtons.Left Then MyBase.OnClick(Nothing)
        MyBase.OnMouseDown(e)
    End Sub

    Protected Overrides Sub OnClick(ByVal e As EventArgs)
    End Sub
#End Region

#Region "Properties"

    Enum ImageAlignments
        Left = 0
        Center = 1
        Right = 2
    End Enum

    Dim _ImageAlignment As ImageAlignments = ImageAlignments.Left
    Property ImageAlignment As ImageAlignments
        Get
            Return _ImageAlignment
        End Get
        Set(ByVal value As ImageAlignments)
            _ImageAlignment = value
            Invalidate()
        End Set
    End Property

    Dim _Image As Image
    Property Image As Image
        Get
            Return _Image
        End Get
        Set(ByVal value As Image)
            _Image = value
            Invalidate()
        End Set
    End Property


    Private _Backgroundcolor1 As Color = Color.FromArgb(60, 63, 80)
    <Category("Appearance DropDown")>
    <Description("Get or Set If the Push Pin is Visible")>
    <DefaultValue(False)>
    Property Backgroundcolor1 As Color
        Get
            Return _Backgroundcolor1
        End Get
        Set(value As Color)
            _Backgroundcolor1 = value
            Invalidate()
        End Set
    End Property

    Private _Backgroundcolor2 As Color = Color.FromArgb(60, 63, 80)
    <Category("Appearance DropDown")>
    <Description("Get or Set the DropDown BackColor")>
    <DefaultValue(GetType(Color), "WhiteSmoke")>
    Property Backgroundcolor2 As Color
        Get
            Return _Backgroundcolor2
        End Get
        Set(value As Color)
            _Backgroundcolor2 = value
            Invalidate()
        End Set
    End Property

    Private _BackgroundcolorOver1 As Color = Color.FromArgb(242, 93, 89)
    Property BackgroundcolorOver1 As Color
        Get
            Return _BackgroundcolorOver1
        End Get
        Set(value As Color)
            _BackgroundcolorOver1 = value
            Invalidate()
        End Set
    End Property

    Private _BackgroundcolorOver2 As Color = Color.FromArgb(242, 93, 89)
    Property BackgroundcolorOver2 As Color
        Get
            Return _BackgroundcolorOver2
        End Get
        Set(value As Color)
            _BackgroundcolorOver2 = value
            Invalidate()
        End Set
    End Property

    Private _BackgroundcolorOnClick1 As Color = Color.FromArgb(142, 33, 11)
    Property BackgroundcolorOnClick1 As Color
        Get
            Return _BackgroundcolorOnClick1
        End Get
        Set(value As Color)
            _BackgroundcolorOnClick1 = value
            Invalidate()
        End Set
    End Property
    Private _BackgroundcolorOnClick2 As Color = Color.FromArgb(142, 33, 11)
    Property BackgroundcolorOnClick2 As Color
        Get
            Return _BackgroundcolorOnClick2
        End Get
        Set(value As Color)
            _BackgroundcolorOnClick2 = value
            Invalidate()
        End Set
    End Property

    Private _Corner As Integer = 10
    Property Corner As Integer
        Get
            Return _Corner
        End Get
        Set(value As Integer)
            If _Corner = 0 Then
                _Corner = 1
            End If


            _Corner = value
            Invalidate()
        End Set
    End Property
#End Region

    Sub New()
        Size = New Size(120, 31)
        DoubleBuffered = True
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.SupportsTransparentBackColor, True)
        BackColor = Color.Transparent
        Font = New Font("Segoe UI", 9)
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        e.Graphics.SmoothingMode = SmoothingMode.HighQuality

        Dim LGB As Brush
        Dim CornerRoud As Integer
        Select Case State
            Case MouseStates.None
                LGB = New LinearGradientBrush(New Rectangle(0, 0, Width - 1, Height - 1), Backgroundcolor1, Backgroundcolor2, 90.0F)
            Case MouseStates.Over
                LGB = New LinearGradientBrush(New Rectangle(0, 0, Width - 1, Height - 1), BackgroundcolorOver1, _BackgroundcolorOver2, 90.0F)
            Case Else
                LGB = New LinearGradientBrush(New Rectangle(0, 0, Width - 1, Height - 1), BackgroundcolorOnClick1, BackgroundcolorOnClick2, 90.0F)
        End Select
        If Not Enabled Then
            LGB = New SolidBrush(Color.FromArgb(0, 0, 0))
        End If



        If Corner = 0 Then
            CornerRoud = 1
        Else
            CornerRoud = Corner
        End If
        If Corner > 35 Then
            CornerRoud = 35
        Else
            CornerRoud = Corner
        End If

        e.Graphics.FillPath(LGB, CreateRound(0, 0, Width - 1, Height - 1, CornerRoud))

        If IsNothing(_Image) Then
            e.Graphics.DrawString(Text, Font, Brushes.White, New Rectangle(3, 2, Width - 7, Height - 5), New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center, .Trimming = StringTrimming.EllipsisCharacter})
        Else
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
            Select Case _ImageAlignment
                Case ImageAlignments.Left
                    Dim ImageRect As New Rectangle(9, 6, Height - 13, Height - 13)
                    e.Graphics.DrawImage(_Image, ImageRect)
                    e.Graphics.DrawString(Text, Font, Brushes.White, New Rectangle(ImageRect.X + ImageRect.Width + 6, 2, Width - ImageRect.Width - 22, Height - 5), New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center, .Trimming = StringTrimming.EllipsisCharacter})
                Case ImageAlignments.Center
                    Dim ImageRect As New Rectangle(((Width - 1) / 2) - (Height - 13) / 2, 6, Height - 13, Height - 13)
                    e.Graphics.DrawImage(_Image, ImageRect)
                Case ImageAlignments.Right
                    Dim ImageRect As New Rectangle(Width - Height + 3, 6, Height - 13, Height - 13)
                    e.Graphics.DrawImage(_Image, ImageRect)
                    e.Graphics.DrawString(Text, Font, Brushes.White, New Rectangle(3, 2, Width - ImageRect.Width - 22, Height - 5), New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center, .Trimming = StringTrimming.EllipsisCharacter})
            End Select
        End If

        MyBase.OnPaint(e)
    End Sub
    Public Sub PerformClick()
        MyBase.OnClick(Nothing)
    End Sub
End Class
#End Region

#Region "UIRaundPicture"
Public Class UIRaundPicture
    Inherits PictureBox
    Dim profile As Image
    Property Image() As Image
        Get
            Return profile
        End Get
        Set(value As Image)
            profile = value
            Invalidate()
        End Set
    End Property
    Dim _Back As Color = BackColor
    <Browsable(True)>
    <Description("Change the background color of the RoundPicturebox.")>
    Property backgroundcolor As Color
        Get
            Return _Back
        End Get
        Set(value As Color)
            _Back = value
        End Set
    End Property
    Dim _PictureWidth As Integer = 64
    <Browsable(True)>
    <Description("Image Width.")>
    Property PictureWidth() As Integer
        Get
            Return _PictureWidth
        End Get
        Set(value As Integer)
            _PictureWidth = value
        End Set
    End Property
    Dim _PictureHeight As Integer = 64
    <Browsable(True)>
    <Description("Image Height.")>
    Property PictureHeight() As Integer
        Get
            Return _PictureHeight
        End Get
        Set(value As Integer)
            _PictureHeight = value
        End Set
    End Property
    Protected Overrides Sub OnPaintBackground(e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaintBackground(e)
        e.Graphics.Clear(backgroundcolor)
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias
        If profile Is Nothing Then
            Return
        End If
        profile = New Bitmap(profile, New Size(PictureWidth, PictureHeight))
        Dim Im As Image = profile
        Using tb As New TextureBrush(Im)
            tb.TranslateTransform(5, 5)
            Using p As New GraphicsPath
                p.AddEllipse(5, 5, profile.Width, profile.Width)
                e.Graphics.FillPath(tb, p)
            End Using
        End Using

        MyBase.OnPaint(e)
    End Sub
    Private Sub InitializeComponent()
        CType(Me, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        CType(Me, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
    End Sub
End Class

#End Region

#Region "UITextbox"
<DefaultEvent("TextChanged")>
Public Class UITextbox
    Inherits Control
    Friend XRailsTB As New TextBox()
    Private borderColor As Pen
    Private _WatermarkContainer As Panel
    Private _WatermarkBrush As SolidBrush
    Private _ColorBordersOnEnter As Boolean = True
    <Browsable(True)>
    <Description("Decides whether the top and bottom border lines are recolored on Enter event.")>
    Public Property ColorBordersOnEnter() As Boolean
        Get
            Return _ColorBordersOnEnter
        End Get
        Set
            _ColorBordersOnEnter = Value
        End Set
    End Property
    Private _Image As Image
    <Browsable(True)>
    <Description("The image displayed in the TextBox.")>
    Public Property Image() As Image
        Get
            Return _Image
        End Get
        Set
            _Image = Value
            _ImageSize = If(Value Is Nothing, Size.Empty, Value.Size)
            XRailsTB.Location = New Point(24, 14)
            Invalidate()
        End Set
    End Property
    Private _ImageSize As Size
    Protected ReadOnly Property ImageSize() As Size
        Get
            Return _ImageSize
        End Get
    End Property
    Private _MaxLength As Integer = 32767
    <Browsable(True)>
    <Description("Specifies the maximum number of characters that can be entered into the edit control.")>
    Public Property MaxLength() As Integer
        Get
            Return _MaxLength
        End Get
        Set
            _MaxLength = Value
            XRailsTB.MaxLength = MaxLength
            Invalidate()
        End Set
    End Property
    Private _Multiline As Boolean
    <Browsable(True)>
    <Description("Controls whether the text of the edit control can span more than one line.")>
    Public Property Multiline() As Boolean
        Get
            Return _Multiline
        End Get
        Set
            _Multiline = Value
            If XRailsTB IsNot Nothing Then
                XRailsTB.Multiline = Value
                If Value Then
                    XRailsTB.Height = Height - 10
                Else
                    Height = XRailsTB.Height + 10
                End If
            End If
        End Set
    End Property
    Private _ReadOnly As Boolean
    <Browsable(True)>
    <Description("Controls whether the text in the edit control can be changed or not.")>
    Public Property [ReadOnly]() As Boolean
        Get
            Return _ReadOnly
        End Get
        Set
            _ReadOnly = Value
            If XRailsTB IsNot Nothing Then
                XRailsTB.[ReadOnly] = Value
            End If
        End Set
    End Property
    Private _ShortcutsEnabled As Boolean = True
    <Browsable(True)>
    <Description("Indicates whether shortcuts defined for the control are enabled.")>
    Public Property ShortcutsEnabled() As Boolean
        Get
            Return _ShortcutsEnabled
        End Get
        Set
            _ShortcutsEnabled = Value
            XRailsTB.ShortcutsEnabled = Value
        End Set
    End Property
    Private _ShowBottomBorder As Boolean = True
    <Browsable(True)>
    <Description("Decides whether the bottom border line should be drawn.")>
    Public Property ShowBottomBorder() As Boolean
        Get
            Return _ShowBottomBorder
        End Get
        Set
            _ShowBottomBorder = Value
            Invalidate()
        End Set
    End Property
    Private _ShowTopBorder As Boolean = False
    <Browsable(True)>
    <Description("Decides whether the top border line should be drawn.")>
    Public Property ShowTopBorder() As Boolean
        Get
            Return _ShowTopBorder
        End Get
        Set
            _ShowTopBorder = Value
            Invalidate()
        End Set
    End Property
    Private _TextAlignment As HorizontalAlignment
    <Browsable(True)>
    <Description("Indicates how the text should be aligned for edit controls.")>
    Public Property TextAlignment() As HorizontalAlignment
        Get
            Return _TextAlignment
        End Get
        Set
            _TextAlignment = Value
            Invalidate()
        End Set
    End Property
    Private _UseSystemPasswordChar As Boolean = False
    <Browsable(True)>
    <Description("Indicates if the text in the edit control should appear as the default password character.")>
    Public Property UseSystemPasswordChar() As Boolean
        Get
            Return _UseSystemPasswordChar
        End Get
        Set
            _UseSystemPasswordChar = Value
            XRailsTB.UseSystemPasswordChar = UseSystemPasswordChar
            Invalidate()
        End Set
    End Property
    Private _Watermark As String = String.Empty
    <Browsable(True)>
    <Description("Allows adding a watermark to the TextBox field when it is empty.")>
    Public Property Watermark() As String
        Get
            Return _Watermark
        End Get
        Set
            _Watermark = Value
            Invalidate()
        End Set
    End Property
    Private _WatermarkColor As Color
    <Browsable(True)>
    <Description("Allows adding a watermark to the TextBox field when it is empty.")>
    Public Property WatermarkColor() As Color
        Get
            Return _WatermarkColor
        End Get
        Set
            _WatermarkColor = Value
            Invalidate()
        End Set
    End Property
    Dim _NormalBorderColor As Color = Color.FromArgb(60, 63, 80)
    Public Property NormalBorderColor As Color
        Get
            Return _NormalBorderColor
        End Get
        Set(value As Color)
            _NormalBorderColor = value
            borderColor = New Pen(value)
            Refresh()
        End Set
    End Property
    Dim _OverBorderColor As Color = Color.FromArgb(242, 93, 89)
    Public Property OverBorderColor As Color
        Get
            Return _OverBorderColor
        End Get
        Set(value As Color)
            _OverBorderColor = value
        End Set
    End Property
    Private Sub _Click(sender As Object, e As EventArgs)
        OnClick(e)
    End Sub
    Private Sub _Enter(sender As Object, e As EventArgs)
        If _ColorBordersOnEnter Then
            borderColor = New Pen(OverBorderColor)
        End If
        _WatermarkBrush = New SolidBrush(_WatermarkColor)
        If XRailsTB.TextLength <= 0 Then
            RemoveWatermark()
            DrawWatermark()
        End If
        Invalidate()
    End Sub
    Private Sub _Leave(sender As Object, e As EventArgs)
        If _ColorBordersOnEnter Then
            borderColor = New Pen(NormalBorderColor)
        End If
        _WatermarkBrush = New SolidBrush(_WatermarkColor)
        If XRailsTB.TextLength <= 0 Then
            RemoveWatermark()
        Else
            Invalidate()
        End If
        Invalidate()
    End Sub
    Private Sub _KeyDown(sender As Object, e As KeyEventArgs)
        If e.Control AndAlso e.KeyCode = Keys.A Then
            XRailsTB.SelectAll()
            e.SuppressKeyPress = True
        End If
        If e.Control AndAlso e.KeyCode = Keys.C Then
            XRailsTB.Copy()
            e.SuppressKeyPress = True
        End If
        OnKeyDown(e)
    End Sub
    Private Sub _KeyUp(sender As Object, e As KeyEventArgs)
        OnKeyUp(e)
    End Sub
    Private Sub _KeyPress(sender As Object, e As KeyPressEventArgs)
        OnKeyPress(e)
    End Sub
    Private Sub WatermarkContainer_Click(sender As Object, e As EventArgs)
        XRailsTB.Focus()
    End Sub
    Private Sub WatermarkContainer_Paint(sender As Object, e As PaintEventArgs)
        _WatermarkContainer.Location = New Point(1, -1)
        _WatermarkContainer.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        _WatermarkContainer.Width = XRailsTB.Width - 25
        _WatermarkContainer.Height = XRailsTB.Height
        XRailsTB.BackColor = BackColor
        _WatermarkBrush = New SolidBrush(_WatermarkColor)
        e.Graphics.DrawString(_Watermark, Font, _WatermarkBrush, New PointF(-3.0F, 1.0F))
    End Sub
    Protected Overrides Sub OnFontChanged(e As EventArgs)
        MyBase.OnFontChanged(e)
        XRailsTB.Font = Font
    End Sub
    Protected Overrides Sub OnForeColorChanged(e As EventArgs)
        MyBase.OnForeColorChanged(e)
        XRailsTB.ForeColor = ForeColor
        Invalidate()
    End Sub
    Protected Overrides Sub OnGotFocus(e As EventArgs)
        MyBase.OnGotFocus(e)
        XRailsTB.Focus()
    End Sub
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        MyBase.OnPaintBackground(e)
        XRailsTB.BackColor = BackColor
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If _Multiline Then
            XRailsTB.Height = Height - 30
        Else
            Height = XRailsTB.Height + 20
        End If
    End Sub
    Protected Overrides Sub OnTextChanged(e As EventArgs)
        MyBase.OnTextChanged(e)
        XRailsTB.Text = Text
    End Sub
    Protected Overrides Sub OnInvalidated(e As InvalidateEventArgs)
        MyBase.OnInvalidated(e)
        If _WatermarkContainer IsNot Nothing Then
            _WatermarkContainer.Invalidate()
        End If
    End Sub
    Public Sub New()
        SetStyle(ControlStyles.SupportsTransparentBackColor Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint, True)
        DoubleBuffered = True
        _WatermarkColor = ColorTranslator.FromHtml("#747881")
        _WatermarkBrush = New SolidBrush(_WatermarkColor)
        _WatermarkContainer = Nothing
        AddTextBox()
        Controls.Add(XRailsTB)
        DrawWatermark()
        'aqui
        borderColor = New Pen(NormalBorderColor)
        Text = Nothing
        Font = New Font("Segoe UI", 10)
        Size = New Size(145, 49)
    End Sub
    Private Sub AddTextBox()
        XRailsTB.Size = New Size(Width - 10, 49)
        XRailsTB.Location = New Point(24, 24)
        XRailsTB.Text = String.Empty
        XRailsTB.BorderStyle = BorderStyle.None
        XRailsTB.TextAlign = HorizontalAlignment.Left
        XRailsTB.Font = New Font("Segoe UI", 9)
        XRailsTB.UseSystemPasswordChar = UseSystemPasswordChar
        XRailsTB.ShortcutsEnabled = ShortcutsEnabled
        XRailsTB.Multiline = False
        ForeColor = ColorTranslator.FromHtml("#7F838C")
        AddHandler XRailsTB.TextChanged, AddressOf _TextChanged
        AddHandler XRailsTB.KeyDown, AddressOf _KeyDown
        AddHandler XRailsTB.KeyPress, AddressOf _KeyPress
        AddHandler XRailsTB.KeyUp, AddressOf _KeyUp
        AddHandler XRailsTB.Click, AddressOf _Click
        AddHandler XRailsTB.Enter, AddressOf _Enter
        AddHandler XRailsTB.Leave, AddressOf _Leave
    End Sub
    Private Sub DrawWatermark()
        If _WatermarkContainer Is Nothing AndAlso XRailsTB.TextLength <= 0 Then
            _WatermarkContainer = New Panel()
            AddHandler _WatermarkContainer.Paint, AddressOf WatermarkContainer_Paint
            _WatermarkContainer.Invalidate()
            AddHandler _WatermarkContainer.Click, AddressOf WatermarkContainer_Click
            XRailsTB.Controls.Add(_WatermarkContainer)
        End If
    End Sub
    Private Sub RemoveWatermark()
        If _WatermarkContainer IsNot Nothing Then
            XRailsTB.Controls.Remove(_WatermarkContainer)
            _WatermarkContainer = Nothing
        End If
    End Sub
    Public Sub _TextChanged(sender As Object, e As EventArgs)
        Text = XRailsTB.Text

        If XRailsTB.TextLength > 0 Then
            RemoveWatermark()
        Else
            DrawWatermark()
        End If
    End Sub

    Public Sub _BaseTextChanged(sender As Object, e As EventArgs)
        XRailsTB.Text = Text
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        Dim bitmap = New Bitmap(Width, Height)
        Dim g = Graphics.FromImage(bitmap)

        DrawWatermark()
        g.SmoothingMode = SmoothingMode.None


        If Image Is Nothing Then
            XRailsTB.Width = Width - 35
        Else
            XRailsTB.Location = New Point(48, XRailsTB.Location.Y)
            XRailsTB.Width = Width - 59
        End If

        XRailsTB.TextAlign = TextAlignment
        XRailsTB.UseSystemPasswordChar = UseSystemPasswordChar

        ' Top border
        If _ShowTopBorder Then
            g.DrawLine(borderColor, 0, 0, Width - 1, 0)
            g.DrawLine(borderColor, 0, 1, Width - 1, 1)
        End If

        ' Bottom border
        If _ShowBottomBorder Then
            g.DrawLine(borderColor, 0, Height - 2, Width - 1, Height - 2)
            g.DrawLine(borderColor, 0, Height - 1, Width - 1, Height - 1)
        End If

        If Image IsNot Nothing Then
            g.DrawImage(_Image, 23, 14, 16, 16)
        End If

        e.Graphics.DrawImage(DirectCast(bitmap.Clone(), Image), 0, 0)

        g.Dispose()
        bitmap.Dispose()
    End Sub
End Class

#End Region

#Region "UIToolTipText"
Public Class UIToolTipText
    Inherits ToolTip
    Protected G As Graphics
    Public Function ColorFromHex(Hex As String) As Color
        Return Color.FromArgb(CInt(Long.Parse(String.Format("FFFFFFFFFF{0}", Hex.Substring(1)), Globalization.NumberStyles.HexNumber)))
    End Function
    Public _ToolTipBackColor As Color = Color.FromArgb(255, 128, 128)
    Property ToolTipBackColor() As Color
        Get
            Return _ToolTipBackColor
        End Get
        Set(value As Color)
            _ToolTipBackColor = value

        End Set
    End Property

    Public _ToolTipBorderColor As Color = Color.FromArgb(255, 255, 255)
    Property ToolTipBorderColor As Color
        Get
            Return _ToolTipBorderColor
        End Get
        Set(value As Color)
            _ToolTipBorderColor = value
        End Set
    End Property
    Private _ToolTipForeColor As Color = Color.FromArgb(255, 255, 255)
    Property ToolTipForeColor As Color
        Get
            Return _ToolTipForeColor
        End Get
        Set(value As Color)
            _ToolTipForeColor = value
        End Set
    End Property

    Private _ToolTipForeTitleInfo As Color = Color.FromArgb(60, 63, 80, 255)
    Property ToolTipForeColorTitleInfo As Color
        Get
            Return _ToolTipForeTitleInfo
        End Get
        Set(value As Color)
            _ToolTipForeTitleInfo = value
        End Set
    End Property

    Private _ToolTipForeColorTitleWarning As Color = Color.FromArgb(255, 242, 157)
    Property ToolTipForeColorTitleWarning As Color
        Get
            Return _ToolTipForeColorTitleWarning
        End Get
        Set(value As Color)
            _ToolTipForeColorTitleWarning = value
        End Set
    End Property

    Private _ToolTipForeColorTitleError As Color = Color.FromArgb(244, 57, 85)
    Property ToolTipForeTitleError As Color
        Get
            Return _ToolTipForeColorTitleError
        End Get
        Set(value As Color)
            _ToolTipForeColorTitleError = value
        End Set
    End Property

    Public Sub New()
        OwnerDraw = True
        BackColor = ColorFromHex("#FFFFFF")
        AddHandler Draw, AddressOf OnDraw
    End Sub

    Private Sub OnDraw(sender As Object, e As DrawToolTipEventArgs)
        G = e.Graphics
        G.SmoothingMode = SmoothingMode.HighQuality
        G.TextRenderingHint = TextRenderingHint.ClearTypeGridFit
        'Color de fondo
        G.Clear(ToolTipBackColor)
        'border
        Using Border As New Pen(ToolTipBorderColor)
            G.DrawRectangle(Border, New Rectangle(e.Bounds.X, e.Bounds.Y, e.Bounds.Width - 1, e.Bounds.Height - 1))
        End Using
        If ToolTipIcon = ToolTipIcon.None Then
            'color texto
            Using TextFont As New Font("Segoe UI", 9), TextBrush As New SolidBrush(ToolTipForeColor)
                G.DrawString(e.ToolTipText, TextFont, TextBrush, New PointF(e.Bounds.X + 5, e.Bounds.Y + 1))
            End Using
        Else
            Select Case ToolTipIcon

                Case ToolTipIcon.Info
                    Using TextFont As New Font("Segoe UI", 9, FontStyle.Bold), TextBrush As New SolidBrush(ToolTipForeColorTitleInfo)
                        G.DrawString("Information", TextFont, TextBrush, New PointF(e.Bounds.X + 4, e.Bounds.Y + 2))
                    End Using

                'Using I As Image = Image.FromStream(New IO.MemoryStream(Convert.FromBase64String(B64)))
                '    G.DrawImage(I, New Rectangle(4, 20 / 2 - 8, 16, 16))
                'End Using
                Case ToolTipIcon.Warning
                    Using TextFont As New Font("Segoe UI", 9, FontStyle.Bold), TextBrush As New SolidBrush(ToolTipForeColorTitleWarning)
                        G.DrawString("Warning", TextFont, TextBrush, New PointF(e.Bounds.X + 4, e.Bounds.Y + 2))
                    End Using
                Case ToolTipIcon.Error
                    Using TextFont As New Font("Segoe UI", 9, FontStyle.Bold), TextBrush As New SolidBrush(ToolTipForeTitleError)
                        G.DrawString("Error", TextFont, TextBrush, New PointF(e.Bounds.X + 4, e.Bounds.Y + 2))
                    End Using
            End Select

            Using TextFont As New Font("Segoe UI", 9), TextBrush As New SolidBrush(ToolTipForeColor)
                G.DrawString(e.ToolTipText, TextFont, TextBrush, New PointF(e.Bounds.X + 4, e.Bounds.Y + 15))
            End Using

        End If

    End Sub

End Class

#End Region

Public Class UIShortcutButton

    Inherits System.Windows.Forms.Control
    Private _Text As String = ""
    <Description("The text that will display as the caption."), Category("Appearance"), DefaultValue("DividerLabel")>
    Public Overrides Property Text() As String
        Get
            Return Me._Text
        End Get
        Set(value As String)
            Me._Text = value
            Me.Invalidate()
        End Set
    End Property

    Private _Image As System.Drawing.Image
    Public Property Image As System.Drawing.Image
        Get
            Return _Image
        End Get
        Set(value As System.Drawing.Image)
            If _Image IsNot value Then
                _Image = value
                Me.Invalidate()
            End If
        End Set
    End Property
    Public Property HoverColor As System.Drawing.Color = Color.DarkRed

    'Private _Title As String = "Title"
    '<DefaultValue("Title")> _
    'Public Property Title As String
    '    Get
    '        Return _Title
    '    End Get
    '    Set(value As String)
    '        If _Title <> value Then
    '            _Title = value
    '            Me.Invalidate()
    '        End If
    '    End Set
    'End Property

    Private Sub _PreviewKeyDown(sender As System.Object, e As System.Windows.Forms.PreviewKeyDownEventArgs) Handles Me.PreviewKeyDown
        If e.KeyCode = Keys.Space Or e.KeyCode = Keys.Enter Then
            Me.PerformClick()
        End If
    End Sub

    Public Sub PerformClick()
        MyBase.InvokeOnClick(Me, New EventArgs)
    End Sub

    Private _Badge As String = "0"
    <DefaultValue("0")>
    Public Property Badge As String
        Get
            Return _Badge
        End Get
        Set(value As String)
            If _Badge <> value Then
                _Badge = value
                Me.Invalidate()
            End If
        End Set
    End Property
    Private _ShowBadge As Boolean = False
    <DefaultValue(False)>
    Public Property ShowBadge As Boolean
        Get
            Return _ShowBadge
        End Get
        Set(value As Boolean)
            If _ShowBadge <> value Then
                _ShowBadge = value
                Me.Invalidate()
            End If
        End Set
    End Property

    Private _BadgeColor As System.Drawing.Color
    Public Property BadgeColor As System.Drawing.Color
        Get
            Return _BadgeColor
        End Get
        Set(value As System.Drawing.Color)
            If _BadgeColor <> value Then
                _BadgeColor = value
                Me.Invalidate()
            End If
        End Set
    End Property

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim _Original As SizeF = e.Graphics.MeasureString(Me.Text, New System.Drawing.Font(Me.Font.FontFamily, 9))

        Dim _FinalText As String = Me.Text
        Dim _Final As SizeF = e.Graphics.MeasureString(_FinalText, New System.Drawing.Font(Me.Font.FontFamily, 9))

        Do While (_Final.Width) + (Me.Padding.Left + Me.Padding.Right) > Me.Width
            _FinalText = _FinalText.Substring(0, _FinalText.Length - IIf(_FinalText.EndsWith("..."), 4, 1)) & "..."
            _Final = e.Graphics.MeasureString(_FinalText, New System.Drawing.Font(Me.Font.FontFamily, 9))
        Loop

        e.Graphics.DrawString(_FinalText, New System.Drawing.Font(Me.Font.FontFamily, 9),
                              New System.Drawing.SolidBrush(IIf(Me.BackColor.GetBrightness >= 0.5, Me.ForeColor, System.Drawing.Color.White)),
                              New System.Drawing.Point((Me.Width - _Final.Width) / 2, ((Me.Height + _Final.Height + 45) / 2) - 6 - Me.Padding.Bottom))

        If Me.Image IsNot Nothing Then
            e.Graphics.DrawImage(Me.Image, CInt((Me.Width - 45) / 2), CInt((Me.Height - 45) / 2) - (_Final.Height / 2), 45, 45)
        End If

        If Me.ShowBadge Then

            Dim _BadgeTemp As String = Me.Badge

            Dim _mb As SizeF = e.Graphics.MeasureString(_BadgeTemp, New System.Drawing.Font(Me.Font.FontFamily, 9))
            Dim _w As Integer = IIf(_mb.Width > 14, _mb.Width, 14) + 6

            Dim _badgerect As New System.Drawing.Rectangle(Me.Width - _w + 3, 0, _w, 20)
            e.Graphics.FillRectangle(New System.Drawing.SolidBrush(Me.BadgeColor), _badgerect)

            e.Graphics.DrawString(_BadgeTemp, New System.Drawing.Font(Me.Font.FontFamily, 9),
                           New System.Drawing.SolidBrush(IIf(Me.BadgeColor.GetBrightness >= 0.5, Me.ForeColor, System.Drawing.Color.White)),
                           New System.Drawing.Point(Me.Width - (_badgerect.Width / 2) - (_mb.Width / 2) + 1, 1))

        End If

    End Sub

    Sub New()

        Me.Size = New System.Drawing.Size(92, 92)
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(243, Byte), Integer), CType(CType(243, Byte), Integer), CType(CType(243, Byte), Integer))

    End Sub

    Private Sub Shortcut_MouseClick(sender As Object, e As MouseEventArgs) Handles Me.MouseClick
        Me.InvokeGotFocus(Me, New EventArgs)
    End Sub

    Private Sub Shortcut_MouseDown(sender As Object, e As MouseEventArgs) Handles Me.MouseDown
        Me.Left += 1
        Me.Top += 1
    End Sub

    Dim _pressed As Boolean = False
    Private Sub Shortcut_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown
        If (e.KeyCode = Keys.Space Or e.KeyCode = Keys.Enter) AndAlso Not _pressed Then
            Me.Left += 1
            Me.Top += 1
            _pressed = True
        End If
    End Sub
    Private Sub Shortcut_MouseUp(sender As Object, e As MouseEventArgs) Handles Me.MouseUp
        Me.Left -= 1
        Me.Top -= 1
    End Sub
    Private Sub Shortcut_KeyUp(sender As Object, e As KeyEventArgs) Handles Me.KeyUp
        If (e.KeyCode = Keys.Space Or e.KeyCode = Keys.Enter) AndAlso _pressed Then
            Me.Left -= 1
            Me.Top -= 1
            _pressed = False
        End If
    End Sub
    Private Sub Shortcut_MouseEnter(sender As Object, e As EventArgs) Handles Me.MouseEnter, Me.GotFocus
        Me.CreateGraphics.DrawLine(New System.Drawing.Pen(Me.HoverColor, 2), 0, Me.Height - 1, Me.Width, Me.Height - 1)
    End Sub

    Private Sub Shortcut_MouseLeave(sender As Object, e As EventArgs) Handles Me.MouseLeave, Me.LostFocus
        Me.Invalidate()
    End Sub

End Class

#Region " Social Button "
Class UISocialButton
    Inherits Control
#Region " Variables "
    Private _Image As Image
    Private _ImageSize As Size
    Private EllipseColor As Color

#End Region
#Region " Properties "

    Property Image() As Image
        Get
            Return _Image
        End Get
        Set(ByVal value As Image)
            If value Is Nothing Then
                _ImageSize = Size.Empty
            Else
                _ImageSize = value.Size
            End If

            _Image = value
            Invalidate()
        End Set
    End Property
    Private _Backgroundcolor As Color = Color.FromArgb(60, 63, 80)
    Property Backgroundcolor As Color
        Get
            Return _Backgroundcolor
        End Get
        Set(value As Color)
            _Backgroundcolor = value
            EllipseColor = value
        End Set
    End Property
    Protected ReadOnly Property ImageSize() As Size
        Get
            Return _ImageSize
        End Get
    End Property
    Private _BackgroundColorOnMouseEnter As Color = Color.FromArgb(181, 41, 42)
    Property BackgroundColorOnMouseEnter As Color
        Get
            Return _BackgroundColorOnMouseEnter
        End Get
        Set(value As Color)
            _BackgroundColorOnMouseEnter = value
        End Set
    End Property
    Private _BackgroundColorOnMouseLeave As Color = Color.FromArgb(66, 76, 85)
    Property BackgroundColorOnMouseLeave As Color
        Get
            Return _BackgroundColorOnMouseLeave
        End Get
        Set(value As Color)
            _BackgroundColorOnMouseLeave = value
        End Set
    End Property
    Private _BackgroundColorOnMouseDown As Color = Color.FromArgb(153, 34, 34)
    Property BackgroundColorOnMouseDown As Color
        Get
            Return _BackgroundColorOnMouseDown
        End Get
        Set(value As Color)
            _BackgroundColorOnMouseDown = value
        End Set
    End Property
    Private _BackgroundColorOnMouseUp As Color = Color.FromArgb(181, 41, 42)
    Property BackgroundColorOnMouseUp As Color
        Get
            Return _BackgroundColorOnMouseUp
        End Get
        Set(value As Color)
            _BackgroundColorOnMouseUp = value
        End Set
    End Property
#End Region
#Region " EventArgs "

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        Me.Size = New Size(54, 54)
    End Sub

    Protected Overrides Sub OnMouseEnter(e As EventArgs)
        MyBase.OnMouseEnter(e)
        EllipseColor = BackgroundColorOnMouseEnter
        Refresh()
    End Sub
    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        EllipseColor = Backgroundcolor
        Refresh()
    End Sub

    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        EllipseColor = BackgroundColorOnMouseDown
        Focus()
        Refresh()
    End Sub
    Protected Overrides Sub OnMouseUp(e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        EllipseColor = BackgroundColorOnMouseUp
        Refresh()
    End Sub

#End Region
#Region " Image Designer "

    Private Shared Function ImageLocation(ByVal SF As StringFormat, ByVal Area As SizeF, ByVal ImageArea As SizeF) As PointF
        Dim MyPoint As PointF
        Select Case SF.Alignment
            Case StringAlignment.Center
                MyPoint.X = CSng((Area.Width - ImageArea.Width) / 2)
        End Select

        Select Case SF.LineAlignment
            Case StringAlignment.Center
                MyPoint.Y = CSng((Area.Height - ImageArea.Height) / 2)
        End Select
        Return MyPoint
    End Function

    Private Function GetStringFormat(ByVal _ContentAlignment As ContentAlignment) As StringFormat
        Dim SF As StringFormat = New StringFormat()
        Select Case _ContentAlignment
            Case ContentAlignment.MiddleCenter
                SF.LineAlignment = StringAlignment.Center
                SF.Alignment = StringAlignment.Center
        End Select
        Return SF
    End Function

#End Region

    Sub New()
        DoubleBuffered = True
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        Dim G As Graphics = e.Graphics
        G.Clear(Parent.BackColor)
        G.SmoothingMode = SmoothingMode.HighQuality

        Dim ImgPoint As PointF = ImageLocation(GetStringFormat(ContentAlignment.MiddleCenter), Size, ImageSize)
        G.FillEllipse(New SolidBrush(EllipseColor), New Rectangle(0, 0, 53, 53))

        If Image IsNot Nothing Then
            G.DrawImage(_Image, ImgPoint.X, ImgPoint.Y, ImageSize.Width, ImageSize.Height)
        End If
    End Sub
End Class

#End Region

#Region "Combobox"

Public Module HelperMethods
    Public GP As GraphicsPath
    Public Enum MouseMode As Byte
        NormalMode
        Hovered
        Pushed
    End Enum
    Public Sub DrawImageFromBase64(ByVal G As Graphics, ByVal Base64Image As String, ByVal Rect As Rectangle)
        Dim IM As Image = Nothing
        With G
            Using ms As New System.IO.MemoryStream(Convert.FromBase64String(Base64Image))
                IM = Image.FromStream(ms) : ms.Close()
            End Using
            .DrawImage(IM, Rect)
        End With
    End Sub
    Public Sub FillRoundedPath(ByVal G As Graphics, ByVal C As Color, ByVal Rect As Rectangle, ByVal Curve As Integer,
                                 Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRight As Boolean = True,
                                 Optional ByVal BottomLeft As Boolean = True, Optional ByVal BottomRight As Boolean = True)
        With G
            .FillPath(New SolidBrush(C), RoundRec(Rect, Curve, TopLeft, TopRight, BottomLeft, BottomRight))
        End With
    End Sub
    Public Sub FillRoundedPath(ByVal G As Graphics, ByVal B As Brush, ByVal Rect As Rectangle, ByVal Curve As Integer,
                                 Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRight As Boolean = True,
                                 Optional ByVal BottomLeft As Boolean = True, Optional ByVal BottomRight As Boolean = True)
        With G
            .FillPath(B, RoundRec(Rect, Curve, TopLeft, TopRight, BottomLeft, BottomRight))
        End With
    End Sub
    Public Sub DrawRoundedPath(ByVal G As Graphics, ByVal C As Color, ByVal Size As Single, ByVal Rect As Rectangle, ByVal Curve As Integer,
                                 Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRight As Boolean = True,
                                 Optional ByVal BottomLeft As Boolean = True, Optional ByVal BottomRight As Boolean = True)
        With G
            .DrawPath(New Pen(C, Size), RoundRec(Rect, Curve, TopLeft, TopRight, BottomLeft, BottomRight))
        End With
    End Sub
    Public Function Triangle(ByVal Clr As Color, ByVal P1 As Point, ByVal P2 As Point, ByVal P3 As Point) As Point()
        Return New Point() {P1, P2, P3}
    End Function
    Public Function PenRGBColor(ByVal GR As Graphics, ByVal R As Integer, ByVal G As Integer, ByVal B As Integer, ByVal Size As Single) As Pen
        Return New Pen(Color.FromArgb(R, G, B), Size)
    End Function
    Public Function SolidBrushRGBColor(ByVal R As Integer, ByVal G As Integer, ByVal B As Integer, Optional ByVal A As Integer = 0) As SolidBrush
        Return New SolidBrush(Color.FromArgb(A, R, G, B))
    End Function
    Public Sub CentreString(ByVal G As Graphics, ByVal Text As String, ByVal font As Font, ByVal brush As Brush, ByVal Rect As Rectangle)
        G.DrawString(Text, font, brush, New Rectangle(0, Rect.Y + (Rect.Height / 2) - (G.MeasureString(Text, font).Height / 2) + 0, Rect.Width, Rect.Height), New StringFormat With {.Alignment = StringAlignment.Center})
    End Sub

    Public Sub LeftString(ByVal G As Graphics, ByVal Text As String, ByVal font As Font, ByVal brush As Brush, ByVal Rect As Rectangle)
        G.DrawString(Text, font, brush, New Rectangle(4, Rect.Y + (Rect.Height / 2) - (G.MeasureString(Text, font).Height / 2) + 0, Rect.Width, Rect.Height), New StringFormat With {.Alignment = StringAlignment.Near})
    End Sub

    Public Sub RightString(ByVal G As Graphics, ByVal Text As String, ByVal font As Font, ByVal brush As Brush, ByVal Rect As Rectangle)
        G.DrawString(Text, font, brush, New Rectangle(4, Rect.Y + (Rect.Height / 2) - (G.MeasureString(Text, font).Height / 2), Rect.Width - Rect.Height + 10, Rect.Height), New StringFormat With {.Alignment = StringAlignment.Far})
    End Sub


#Region " Round Border "
    Public Function RoundRec(ByVal r As Rectangle, ByVal Curve As Integer,
                                 Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRight As Boolean = True,
                                 Optional ByVal BottomLeft As Boolean = True, Optional ByVal BottomRight As Boolean = True) As GraphicsPath
        Dim CreateRoundPath As New GraphicsPath(FillMode.Winding)
        If TopLeft Then
            CreateRoundPath.AddArc(r.X, r.Y, Curve, Curve, 180.0F, 90.0F)
        Else
            CreateRoundPath.AddLine(r.X, r.Y, r.X, r.Y)
        End If
        If TopRight Then
            CreateRoundPath.AddArc(r.Right - Curve, r.Y, Curve, Curve, 270.0F, 90.0F)
        Else
            CreateRoundPath.AddLine(r.Right - r.Width, r.Y, r.Width, r.Y)
        End If
        If BottomRight Then
            CreateRoundPath.AddArc(r.Right - Curve, r.Bottom - Curve, Curve, Curve, 0.0F, 90.0F)
        Else
            CreateRoundPath.AddLine(r.Right, r.Bottom, r.Right, r.Bottom)

        End If
        If BottomLeft Then
            CreateRoundPath.AddArc(r.X, r.Bottom - Curve, Curve, Curve, 90.0F, 90.0F)
        Else
            CreateRoundPath.AddLine(r.X, r.Bottom, r.X, r.Bottom)
        End If
        CreateRoundPath.CloseFigure()
        Return CreateRoundPath
    End Function

#End Region
End Module

Public Class UICombobox
    Inherits ComboBox
    Private _StartIndex As Integer = 0
    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        BackColor = Color.FromArgb(255, 255, 255)
        Font = New Font("Segoe UI", 9, FontStyle.Regular)
        DrawMode = Forms.DrawMode.OwnerDrawFixed
        DoubleBuffered = True
        StartIndex = 0
        DropDownHeight = 100
        DropDownStyle = ComboBoxStyle.DropDownList
        UpdateStyles()
    End Sub
    <Category("Behavior"),
    Description("When overridden in a derived class, gets or sets the zero-based index of the currently selected item.")>
    Private Property StartIndex As Integer
        Get
            Return _StartIndex
        End Get
        Set(ByVal value As Integer)
            _StartIndex = value
            Try
                MyBase.SelectedIndex = value
            Catch
            End Try
            Invalidate()
        End Set
    End Property

    Private _backgroundcolor As Color = BackColor
    <Category(" Custom Properties "),
    Description("Gets or sets the background color for the control.")>
    Public Property backgroundcolor As Color
        Get
            Return _backgroundcolor
        End Get
        Set(value As Color)
            _backgroundcolor = value
            Invalidate()
        End Set
    End Property

    Private _LineColor As Color = Color.FromArgb(60, 63, 80)
    <Category(" Custom Properties "),
    Description("Gets or sets the lines color for the control.")>
    Public Property LineColor As Color
        Get
            Return _LineColor
        End Get
        Set(value As Color)
            _LineColor = value
            Invalidate()
        End Set
    End Property

    Private _TextColor As Color = Color.FromArgb(255, 255, 255)
    <Category(" Custom Properties "),
    Description("Gets or sets the text color for the control.")>
    Public Property TextColor As Color
        Get
            Return _TextColor
        End Get
        Set(value As Color)
            _TextColor = value
            Invalidate()
        End Set
    End Property
    Protected Overrides Sub OnDrawItem(e As DrawItemEventArgs)
        Try
            Dim G As Graphics = e.Graphics
            e.DrawBackground()
            With G
                .TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
                .FillRectangle(New SolidBrush(_backgroundcolor), e.Bounds)

                If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
                    Cursor = Cursors.Hand
                    CentreString(G, Items(e.Index), Font, New SolidBrush(TextColor), New Rectangle(e.Bounds.X + 1, e.Bounds.Y + 3, e.Bounds.Width - 2, e.Bounds.Height - 2))
                Else
                    CentreString(G, Items(e.Index), Font, Brushes.DimGray, New Rectangle(e.Bounds.X + 1, e.Bounds.Y + 2, e.Bounds.Width - 2, e.Bounds.Height - 2))
                End If
            End With
        Catch
        End Try
        Invalidate()
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim Down64 As String = "iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAQAAAC1+jfqAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAADdcAAA3XAUIom3gAAAAHdElNRQfhAwIRLizYXnxUAAAAVklEQVQoz7XRuQ2AMBAF0XEZ9EKCoA8kMu4a6QVRhoeECMmbedK3+slC9ZIDbeBX8qEJDm7cLJcdAc8iT9+OR8gA7iEDuIYM4BIygLM5YAB7u/rf+fUCWoOKwMIiR7AAAAAldEVYdGRhdGU6Y3JlYXRlADIwMTctMDMtMDJUMTc6NDY6NDQrMDE6MDCuVGsiAAAAJXRFWHRkYXRlOm1vZGlmeQAyMDE3LTAzLTAyVDE3OjQ2OjQ0KzAxOjAw3wnTngAAABl0RVh0U29mdHdhcmUAd3d3Lmlua3NjYXBlLm9yZ5vuPBoAAAAASUVORK5CYII="
        Dim Rect As New Rectangle(0, 0, Width, Height - 1)
        Dim newImage As Image = Image.FromStream(New IO.MemoryStream(Convert.FromBase64String(Down64)))
        With e.Graphics
            .TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
            .FillRectangle(New SolidBrush(_backgroundcolor), Rect)
            .DrawImage(newImage, Width - 15, 15, 8, 8)
            .DrawLine(New Pen(LineColor, 2), New Point(0, Height - 1), New Point(Width, Height - 1))
            .DrawString(Text, Font, New SolidBrush(TextColor), New Rectangle(5, 1, Width - 1, Height - 1), New StringFormat With {.LineAlignment = StringAlignment.Near, .Alignment = StringAlignment.Near})
        End With
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        Invalidate()
    End Sub
End Class
#End Region

#Region "SwitchButton"
<DefaultEvent("CheckedChanged")> Public Class UISwitch
    Inherits Control

#Region " Variables "

    Private _Checked As Boolean
    Protected State As MouseMode = MouseMode.NormalMode
    Private _UnCheckedColor As Color = Color.FromArgb(242, 93, 89) ' GetHTMLColor("dedede")
    Private _CheckedColor As Color = Color.FromArgb(49, 122, 46) 'GetHTMLColor("3acf5f")
    Private _CheckedBallColor As Color = Color.White
    Private _UnCheckedBallColor As Color = Color.White

#End Region

#Region " Properties "

    <Category("Appearance")>
    Property Checked As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            _Checked = value
            RaiseEvent CheckedChanged(Me)
            Invalidate()
        End Set
    End Property

    <Category("Custom Properties"),
    Description("Gets or sets the switch control color while unchecked")>
    Public Property UnCheckedColor As Color
        Get
            Return _UnCheckedColor
        End Get
        Set(value As Color)
            _UnCheckedColor = value
            Invalidate()
        End Set
    End Property

    <Category("Custom Properties"),
    Description("Gets or sets the switch control color while checked")>
    Public Property CheckedColor As Color
        Get
            Return _CheckedColor
        End Get
        Set(value As Color)
            _CheckedColor = value
            Invalidate()
        End Set
    End Property

    <Category("Custom Properties"),
    Description("Gets or sets the switch control ball color while checked")>
    Public Property CheckedBallColor As Color
        Get
            Return _CheckedBallColor
        End Get
        Set(value As Color)
            _CheckedBallColor = value
            Invalidate()
        End Set
    End Property

    <Category("Custom Properties"),
    Description("Gets or sets the switch control ball color while unchecked")>
    Public Property UnCheckedBallColor As Color
        Get
            Return _UnCheckedBallColor
        End Get
        Set(value As Color)
            _UnCheckedBallColor = value
            Invalidate()
        End Set
    End Property

#End Region

#Region " Constructors "

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
     ControlStyles.SupportsTransparentBackColor Or ControlStyles.UserPaint, True)
        DoubleBuffered = True
        Cursor = Cursors.Hand
        BackColor = Color.Transparent
        UpdateStyles()
    End Sub

#End Region

#Region " Events "

    Event CheckedChanged(ByVal sender As Object)

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        Size = New Size(30, 19)
        Invalidate()
    End Sub

    Protected Overrides Sub OnClick(ByVal e As EventArgs)
        _Checked = Not Checked
        MyBase.OnClick(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As System.EventArgs)
        Invalidate() : MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Sub OnMouseHover(e As EventArgs)
        MyBase.OnMouseHover(e)
        State = MouseMode.Hovered
        Invalidate()
    End Sub

    Protected Overrides Sub OnMouseLeave(e As EventArgs)
        MyBase.OnMouseLeave(e)
        State = MouseMode.NormalMode
        Invalidate()
    End Sub

#End Region

#Region " Draw Control "

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        With e.Graphics
            .SmoothingMode = SmoothingMode.AntiAlias
            If Checked Then
                FillRoundedPath(e.Graphics, New SolidBrush(CheckedColor), New Rectangle(0, 1, 28, 16), 16)
                .FillEllipse(New SolidBrush(CheckedBallColor), New Rectangle(Width - 17, 0, 16, 18))
            Else
                FillRoundedPath(e.Graphics, New SolidBrush(UnCheckedColor), New Rectangle(0, 1, 28, 16), 16)
                .FillEllipse(New SolidBrush(UnCheckedBallColor), New Rectangle(0.5, 0, 16, 18))
            End If

        End With
    End Sub

#End Region

End Class
#End Region

#Region "RadioButton"

#Region " RadioButton "

<DefaultEvent("CheckedChanged")> Public Class UIRadioButton
    Inherits Control

#Region " Variables "

    Private _Checked As Boolean
    Protected _Group As Integer = 1
    Protected State As MouseMode = MouseMode.NormalMode
    Private _CheckBorderColor As Color = Color.FromArgb(255, 255, 255)
    Private _UnCheckBorderColor As Color = Color.FromArgb(242, 93, 89)
    Private _CheckColor As Color = Color.FromArgb(242, 93, 89)

#End Region

#Region " Events "

    Event CheckedChanged(ByVal sender As Object)

#End Region

#Region " Properties "

    <Category("Appearance")>
    Property Checked As Boolean
        Get
            Return _Checked
        End Get
        Set(ByVal value As Boolean)
            _Checked = value
            RaiseEvent CheckedChanged(Me)
            Invalidate()
        End Set
    End Property

    <Category("Appearance")>
    Property Group As Integer
        Get
            Return _Group
        End Get
        Set(ByVal value As Integer)
            _Group = value
            Invalidate()
        End Set
    End Property

    <Category("Custom Properties"),
    Description("Gets or sets the Radiobutton control border color while checked.")>
    Property CheckBorderColor As Color
        Get
            Return _CheckBorderColor
        End Get
        Set(value As Color)
            _CheckBorderColor = value
            Invalidate()
        End Set
    End Property

    <Category("Custom Properties"),
    Description("Gets or sets the Radiobutton control border color while unchecked.")>
    Property UnCheckBorderColor As Color
        Get
            Return _UnCheckBorderColor
        End Get
        Set(value As Color)
            _UnCheckBorderColor = value
            Invalidate()
        End Set
    End Property

    <Category("Custom Properties"),
    Description("Gets or sets the Radiobutton control check symbol color while checked.")>
    Property CheckColor As Color
        Get
            Return _CheckColor
        End Get
        Set(value As Color)
            _CheckColor = value
            Invalidate()
        End Set
    End Property

#End Region

#Region " Constructors "

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or
    ControlStyles.SupportsTransparentBackColor Or ControlStyles.UserPaint, True)
        DoubleBuffered = True
        Cursor = Cursors.Hand
        BackColor = Color.Transparent
        ForeColor = Color.FromArgb(255, 255, 255)
        Font = New Font("Segoe UI", 9, FontStyle.Regular)
        UpdateStyles()
    End Sub

#End Region

#Region " Draw Control "

    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        Dim G As Graphics = e.Graphics
        Dim R As New Rectangle(1, 1, 18, 18)
        With G
            .SmoothingMode = SmoothingMode.HighQuality
            .TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
            If Checked Then
                .FillEllipse(New SolidBrush(CheckColor), New Rectangle(4, 4, 12, 12))
                .DrawEllipse(New Pen(CheckBorderColor, 2), R)
            Else
                .DrawEllipse(New Pen(UnCheckBorderColor, 2), R)
            End If
            .DrawString(Text, Font, New SolidBrush(ForeColor), New Rectangle(21, 1.5, Width, Height - 2), New StringFormat With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Center})
        End With

    End Sub

#End Region

#Region " Events "

    Private Sub UpdateState()
        If Not IsHandleCreated OrElse Not Checked Then Return
        For Each C As Control In Parent.Controls
            If C IsNot Me AndAlso TypeOf C Is UIRadioButton AndAlso DirectCast(C, UIRadioButton).Group = _Group Then
                DirectCast(C, UIRadioButton).Checked = False
            End If
        Next
    End Sub

    Protected Overrides Sub OnClick(ByVal e As EventArgs)
        _Checked = Not Checked
        UpdateState()
        MyBase.OnClick(e)
        Invalidate()
    End Sub

    Protected Overrides Sub OnCreateControl()
        UpdateState()
        MyBase.OnCreateControl()
    End Sub

    Protected Overrides Sub OnTextChanged(ByVal e As System.EventArgs)
        Invalidate() : MyBase.OnTextChanged(e)
    End Sub

    Protected Overrides Sub OnResize(ByVal e As System.EventArgs)
        MyBase.OnResize(e)
        Height = 21
        Invalidate()
    End Sub

#End Region

End Class

#End Region
#End Region

Public Class UIRadialProgressBar
    Inherits Control

#Region "Declarations"
    Private _BorderColour As Color = Color.FromArgb(242, 93, 89)
    Private _BaseColour As Color = Color.FromArgb(242, 93, 89)
    Private _ProgressColour As Color = Color.FromArgb(255, 255, 255)
    Private _Value As Integer = 0
    Private _Maximum As Integer = 100
    Private _StartingAngle As Integer = 110
    Private _RotationAngle As Integer = 355
    Private ReadOnly _Font As Font = New Font("Segoe UI", 20)
#End Region

#Region "Properties"

    <Category("Control")>
    Public Property Maximum() As Integer
        Get
            Return _Maximum
        End Get
        Set(V As Integer)
            Select Case V
                Case Is < _Value
                    _Value = V
            End Select
            _Maximum = V
            Invalidate()
        End Set
    End Property

    <Category("Control")>
    Public Property Value() As Integer
        Get
            Select Case _Value
                Case 0
                    Return 0
                Case Else
                    Return _Value
            End Select
        End Get

        Set(V As Integer)
            Select Case V
                Case Is > _Maximum
                    V = _Maximum
                    Invalidate()
            End Select
            _Value = V
            Invalidate()
        End Set
    End Property

    Public Sub Increment(ByVal Amount As Integer)
        Value += Amount
    End Sub

    <Category("Colours")>
    Public Property BorderColour As Color
        Get
            Return _BorderColour
        End Get
        Set(value As Color)
            _BorderColour = value
        End Set
    End Property

    <Category("Colours")>
    Public Property ProgressColour As Color
        Get
            Return _ProgressColour
        End Get
        Set(value As Color)
            _ProgressColour = value
        End Set
    End Property

    <Category("Colours")>
    Public Property BaseColour As Color
        Get
            Return _BaseColour
        End Get
        Set(value As Color)
            _BaseColour = value
        End Set
    End Property

    <Category("Control")>
    Public Property StartingAngle As Integer
        Get
            Return _StartingAngle
        End Get
        Set(value As Integer)
            _StartingAngle = value
        End Set
    End Property

    <Category("Control")>
    Public Property RotationAngle As Integer
        Get
            Return _RotationAngle
        End Get
        Set(value As Integer)
            _RotationAngle = value
        End Set
    End Property

#End Region

#Region "Draw Control"
    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint Or
                ControlStyles.ResizeRedraw Or ControlStyles.OptimizedDoubleBuffer Or
                ControlStyles.SupportsTransparentBackColor, True)
        DoubleBuffered = True
        Size = New Size(78, 78)
        BackColor = Color.Transparent
    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim B As New Bitmap(Width, Height)
        Dim G = Graphics.FromImage(B)
        With G
            .TextRenderingHint = TextRenderingHint.AntiAliasGridFit
            .SmoothingMode = SmoothingMode.HighQuality
            .PixelOffsetMode = PixelOffsetMode.HighQuality
            .Clear(BackColor)
            Select Case _Value
                Case 0
                    .DrawArc(New Pen(New SolidBrush(_BorderColour), 1 + 5), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle - 3, _RotationAngle + 5)
                    .DrawArc(New Pen(New SolidBrush(_BaseColour), 1 + 3), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle, _RotationAngle)
                    .DrawString(_Value, _Font, Brushes.White, New Point(Width / 2, Height / 2 - 1), New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
                Case _Maximum
                    .DrawArc(New Pen(New SolidBrush(_BorderColour), 1 + 5), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle - 3, _RotationAngle + 5)
                    .DrawArc(New Pen(New SolidBrush(_BaseColour), 1 + 3), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle, _RotationAngle)
                    .DrawArc(New Pen(New SolidBrush(_ProgressColour), 1 + 3), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle, _RotationAngle)
                    .DrawString(_Value, _Font, Brushes.White, New Point(Width / 2, Height / 2 - 1), New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
                Case Else
                    .DrawArc(New Pen(New SolidBrush(_BorderColour), 1 + 5), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle - 3, _RotationAngle + 5)
                    .DrawArc(New Pen(New SolidBrush(_BaseColour), 1 + 3), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle, _RotationAngle)
                    .DrawArc(New Pen(New SolidBrush(_ProgressColour), 1 + 3), CInt(3 / 2) + 1, CInt(3 / 2) + 1, Width - 3 - 4, Height - 3 - 3, _StartingAngle, CInt((_RotationAngle / _Maximum) * _Value))
                    .DrawString(_Value, _Font, Brushes.White, New Point(Width / 2, Height / 2 - 1), New StringFormat With {.Alignment = StringAlignment.Center, .LineAlignment = StringAlignment.Center})
            End Select
        End With
        MyBase.OnPaint(e)
        e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic
        e.Graphics.DrawImageUnscaled(B, 0, 0)
        B.Dispose()
    End Sub
#End Region

End Class

#Region "UILinkLabel"
Public Class UILinkLabel
    Inherits LinkLabel
    Private ReadOnly linkColor As Color = ColorTranslator.FromHtml("#F25D59")
    Private ReadOnly activeLinkColor As Color = ColorTranslator.FromHtml("#DE5954")
    Private Const WM_SETCURSOR As Integer = &H20
    Private Const IDC_HAND As Integer = 32649


    Protected Overrides Sub WndProc(ByRef msg As Message)
        If msg.Msg = WM_SETCURSOR Then
            msg.Result = IntPtr.Zero
            Return
        End If
        MyBase.WndProc(msg)
    End Sub

    Public Sub New()
        Font = New Font("Segoe UI", 9, FontStyle.Regular)
        BackColor = Color.Transparent
        linkColor = linkColor
        activeLinkColor = activeLinkColor
        VisitedLinkColor = activeLinkColor
        LinkBehavior = LinkBehavior.NeverUnderline
        Cursor = Cursors.Arrow
    End Sub
    Protected Overrides Sub OnMouseDown(e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        Focus()
    End Sub
    Protected Overrides Sub OnInvalidated(e As InvalidateEventArgs)
        MyBase.OnInvalidated(e)

        VisitedLinkColor = activeLinkColor
    End Sub
End Class
#End Region

#Region "UISeparator"
Class ContainerObjectDisposable : Implements IDisposable
    Private iList As New List(Of IDisposable)
    Public Sub AddObject(ByVal Obj As IDisposable)
        iList.Add(Obj)
    End Sub
    Public Sub AddObjectRange(ByVal Obj() As IDisposable)
        iList.AddRange(Obj)
    End Sub
#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                For Each ObjectDisposable As IDisposable In iList
                    ObjectDisposable.Dispose()
#If DEBUG Then
                    Debug.WriteLine("Dispose : " & ObjectDisposable.ToString)
#End If
                Next
            End If

        End If
        iList.Clear()
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
Class UISeparator : Inherits Control
    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.DoubleBuffer Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.SupportsTransparentBackColor Or ControlStyles.UserPaint, True)
        Width = 400
        Height = 3
        BackColor = Color.Transparent
    End Sub

#Region "Declarations"
    Private G As Graphics
    Private LGB1, LGB2 As LinearGradientBrush
    Private P1, P2 As Pen
    Private CB1, CB2 As ColorBlend
    Private C1, C2 As Color
    Private conObj As New ContainerObjectDisposable
#End Region 'Declarations
    Private _LineColor1 As Color = BackColor
    Property LineColor1 As Color
        Get
            Return _LineColor1
        End Get
        Set(value As Color)
            _LineColor1 = value
        End Set
    End Property
    Private _LineColor2 As Color = BackColor
    Property LineColor2 As Color
        Get
            Return _LineColor2
        End Get
        Set(value As Color)
            _LineColor2 = value
        End Set
    End Property
    Protected Overrides Sub OnPaint(ByVal e As PaintEventArgs)
        MyBase.OnPaint(e)
        '#Zone " Declarations "
        Dim Colors1, Colors2 As New List(Of Color)

        C1 = LineColor1
        C2 = LineColor2

        G = e.Graphics

        LGB1 = New LinearGradientBrush(New Point(0, 0), New Point(Width, 0), Nothing, Nothing)
        LGB2 = New LinearGradientBrush(New Point(0, 1), New Point(Width, 1), Nothing, Nothing)
        conObj.AddObjectRange({LGB1, LGB2})

        CB1 = New ColorBlend
        CB2 = New ColorBlend
        '#End Zone

        '#Zone " Colors first line "
        For i As Integer = 0 To 255 Step 51
            Colors1.Add(Color.FromArgb(i, C1))
        Next
        For i As Integer = 255 To 0 Step -51
            Colors1.Add(Color.FromArgb(i, C1))
        Next
        '#End Zone

        '#Zone " Colors Second line "
        For i As Integer = 0 To 255 Step 51
            Colors2.Add(Color.FromArgb(i, C2))
        Next
        For i As Integer = 255 To 0 Step -51
            Colors2.Add(Color.FromArgb(i, C2))
        Next
        '#End Zone

        '#Zone " colorblend first line  "
        CB1.Colors = Colors1.ToArray
        CB1.Positions = {0.0, 0.08, 0.16, 0.24, 0.32, 0.4, 0.48, 0.56, 0.64, 0.72, 0.8, 1.0}
        '#End Zone

        '#Zone " colorblend second line "
        CB2.Colors = Colors2.ToArray
        CB2.Positions = {0.0, 0.08, 0.16, 0.24, 0.32, 0.4, 0.48, 0.56, 0.64, 0.72, 0.8, 1.0}
        '#End Zone

        '#Zone " Pen "
        LGB1.InterpolationColors = CB1
        LGB2.InterpolationColors = CB2

        P1 = New Pen(LGB1)
        P2 = New Pen(LGB2)
        conObj.AddObjectRange({P1, P2})
        '#End Zone

        G.DrawLine(P1, 0, 0, Width, 0)
        G.DrawLine(P2, 0, 1, Width, 1)

        conObj.Dispose()
    End Sub
End Class
#End Region

#Region "UITrackBar "
<DefaultEvent("ValueChanged")>
Class UItrackBar
    Inherits Control

#Region " Enums "

    Enum ValueDivisor
        By1 = 1
        By10 = 10
        By100 = 100
        By1000 = 1000
    End Enum

#End Region
#Region " Variables "

    Private PipeBorder, FillValue As GraphicsPath
    Private TrackBarHandleRect As Rectangle
    Private Cap As Boolean
    Private ValueDrawer As Integer

    Private ThumbSize As Size = New Size(15, 15)
    Private TrackThumb As Rectangle

    Private _Minimum As Integer = 0
    Private _Maximum As Integer = 10
    Private _Value As Integer = 0

    Private _DrawValueString As Boolean = False
    Private _JumpToMouse As Boolean = False
    Private DividedValue As ValueDivisor = ValueDivisor.By1

#End Region
#Region " Properties "

    Public Property Minimum() As Integer
        Get
            Return _Minimum
        End Get
        Set(ByVal value As Integer)

            If value >= _Maximum Then value = _Maximum - 10
            If _Value < value Then _Value = value

            _Minimum = value
            Invalidate()
        End Set
    End Property

    Public Property Maximum() As Integer
        Get
            Return _Maximum
        End Get
        Set(ByVal value As Integer)

            If value <= _Minimum Then value = _Minimum + 10
            If _Value > value Then _Value = value

            _Maximum = value
            Invalidate()
        End Set
    End Property

    Event ValueChanged()
    Public Property Value() As Integer
        Get
            Return _Value
        End Get
        Set(ByVal value As Integer)
            If _Value <> value Then
                If value < _Minimum Then
                    _Value = _Minimum
                Else
                    If value > _Maximum Then
                        _Value = _Maximum
                    Else
                        _Value = value
                    End If
                End If
                Invalidate()
                RaiseEvent ValueChanged()
            End If
        End Set
    End Property

    Public Property ValueDivison() As ValueDivisor
        Get
            Return DividedValue
        End Get
        Set(ByVal Value As ValueDivisor)
            DividedValue = Value
            Invalidate()
        End Set
    End Property

    <Browsable(False)> Public Property ValueToSet() As Single
        Get
            Return CSng(_Value / DividedValue)
        End Get
        Set(ByVal Val As Single)
            Value = CInt(Val * DividedValue)
        End Set
    End Property

    Public Property JumpToMouse() As Boolean
        Get
            Return _JumpToMouse
        End Get
        Set(ByVal value As Boolean)
            _JumpToMouse = value
            Invalidate()
        End Set
    End Property

    Property DrawValueString() As Boolean
        Get
            Return _DrawValueString
        End Get
        Set(ByVal value As Boolean)
            _DrawValueString = value
            If _DrawValueString = True Then
                Height = 35
            Else
                Height = 22
            End If
            Invalidate()
        End Set
    End Property
    Private PipeBackColor As Color = Color.FromArgb(60, 63, 80)
    Private PipeBorderColor As Color = Color.FromArgb(60, 63, 80)
    Private PipeFillValue As Color = Color.FromArgb(42, 93, 89)

#End Region
#Region " EventArgs "

    Protected Overrides Sub OnMouseMove(ByVal e As MouseEventArgs)
        MyBase.OnMouseMove(e)
        If Cap = True AndAlso e.X > -1 AndAlso e.X < (Width + 1) Then
            Value = _Minimum + CInt((_Maximum - _Minimum) * (e.X / Width))
        End If
    End Sub
    Public Function RoundRect(ByVal Rectangle As Rectangle, ByVal Curve As Integer) As GraphicsPath
        Dim P As GraphicsPath = New GraphicsPath()
        Dim ArcRectangleWidth As Integer = Curve * 2
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90)
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90)
        P.AddLine(New Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), New Point(Rectangle.X, Curve + Rectangle.Y))
        Return P
    End Function
    Public Function RoundRect(ByVal X As Integer, ByVal Y As Integer, ByVal Width As Integer, ByVal Height As Integer, ByVal Curve As Integer) As GraphicsPath
        Dim Rectangle As Rectangle = New Rectangle(X, Y, Width, Height)
        Dim P As GraphicsPath = New GraphicsPath()
        Dim ArcRectangleWidth As Integer = Curve * 2
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 0, 90)
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), 90, 90)
        P.AddLine(New Point(Rectangle.X, Rectangle.Height - ArcRectangleWidth + Rectangle.Y), New Point(Rectangle.X, Curve + Rectangle.Y))
        Return P
    End Function
    Public Function RoundedTopRect(ByVal Rectangle As Rectangle, ByVal Curve As Integer) As GraphicsPath
        Dim P As GraphicsPath = New GraphicsPath()
        Dim ArcRectangleWidth As Integer = Curve * 2
        P.AddArc(New Rectangle(Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -180, 90)
        P.AddArc(New Rectangle(Rectangle.Width - ArcRectangleWidth + Rectangle.X, Rectangle.Y, ArcRectangleWidth, ArcRectangleWidth), -90, 90)
        P.AddLine(New Point(Rectangle.X + Rectangle.Width, Rectangle.Y + ArcRectangleWidth), New Point(Rectangle.X + Rectangle.Width, Rectangle.Y + Rectangle.Height - 1))
        P.AddLine(New Point(Rectangle.X, Rectangle.Height - 1 + Rectangle.Y), New Point(Rectangle.X, Rectangle.Y + Curve))
        Return P
    End Function
    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        MyBase.OnMouseDown(e)
        If e.Button = Forms.MouseButtons.Left Then
            ValueDrawer = CInt((_Value - _Minimum) / (_Maximum - _Minimum) * (Width - 11))
            TrackBarHandleRect = New Rectangle(ValueDrawer, 0, 25, 25)
            Cap = TrackBarHandleRect.Contains(e.Location)
            Focus()
            If _JumpToMouse Then
                Value = _Minimum + CInt((_Maximum - _Minimum) * (e.X / Width))
            End If
        End If
    End Sub

    Protected Overrides Sub OnMouseUp(ByVal e As MouseEventArgs)
        MyBase.OnMouseUp(e)
        Cap = False
    End Sub

#End Region

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or
             ControlStyles.UserPaint Or
             ControlStyles.ResizeRedraw Or
             ControlStyles.DoubleBuffer, True)

        Size = New Size(80, 22)
        MinimumSize = New Size(47, 22)
    End Sub

    Protected Overrides Sub OnResize(e As EventArgs)
        MyBase.OnResize(e)
        If _DrawValueString = True Then
            Height = 35
        Else
            Height = 22
        End If
    End Sub
    Private _BorderColor As Color = Color.FromArgb(255, 255, 255)
    Public Property BorderColor As Color
        Get
            Return _BorderColor
        End Get
        Set(value As Color)
            _BorderColor = value
        End Set
    End Property
    Private _LineColor As Color = Color.FromArgb(255, 255, 255)
    Public Property LineColor As Color
        Get
            Return _LineColor
        End Get
        Set(value As Color)
            _LineColor = value
        End Set
    End Property
    Private _LineColor2 As Color = Color.FromArgb(255, 255, 255)
    Public Property LineColor2 As Color
        Get
            Return _LineColor2
        End Get
        Set(value As Color)
            _LineColor2 = value
        End Set
    End Property
    Private _Color3 As Color = Color.FromArgb(255, 255, 255)
    Public Property Color3 As Color
        Get
            Return _Color3
        End Get
        Set(value As Color)
            _Color3 = value
        End Set
    End Property
    Protected Overrides Sub OnPaint(e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)
        Dim G As Graphics = e.Graphics

        G.Clear(Parent.BackColor)
        G.SmoothingMode = SmoothingMode.AntiAlias
        TrackThumb = New Rectangle(8, 10, Width - 16, 2)
        PipeBorder = RoundRect(1, 8, Width - 3, 5, 2)

        Try
            ValueDrawer = CInt((_Value - _Minimum) / (_Maximum - _Minimum) * (Width - 11))
        Catch ex As Exception
        End Try

        TrackBarHandleRect = New Rectangle(ValueDrawer, 0, 10, 20)

        G.SetClip(PipeBorder) ' Set the clipping region of this Graphics to the specified GraphicsPath
        'Color de lleno
        G.FillPath(New SolidBrush(LineColor), PipeBorder)
        FillValue = RoundRect(1, 8, TrackBarHandleRect.X + TrackBarHandleRect.Width - 4, 5, 2)


        G.ResetClip() ' Reset the clip region of this Graphics to an infinite region

        G.SmoothingMode = SmoothingMode.HighQuality
        'color de linea o borde
        G.DrawPath(New Pen(BorderColor), PipeBorder) ' Draw pipe border
        G.FillPath(New SolidBrush(LineColor2), FillValue) 'Llleno
        'color de boder de bola
        G.FillEllipse(New SolidBrush(Color3), TrackThumb.X + CInt(TrackThumb.Width * (Value / Maximum)) - CInt(ThumbSize.Width / 2), TrackThumb.Y + CInt((TrackThumb.Height / 2)) - CInt(ThumbSize.Height / 2), ThumbSize.Width, ThumbSize.Height)
        'color de bola
        G.DrawEllipse(New Pen(Color3), TrackThumb.X + CInt(TrackThumb.Width * (Value / Maximum)) - CInt(ThumbSize.Width / 2), TrackThumb.Y + CInt((TrackThumb.Height / 2)) - CInt(ThumbSize.Height / 2), ThumbSize.Width, ThumbSize.Height)

        If _DrawValueString = True Then
            G.DrawString(ValueToSet, Font, Brushes.DimGray, 1, 20)
        End If
    End Sub
End Class



#End Region

#Region " NotificationBox "

Class UINotificationBox
    Inherits Control

#Region " Variables "

    Private CloseCoordinates As Point
    Private IsOverClose As Boolean
    Private _BorderCurve As Integer = 8
    Private CreateRoundPath As GraphicsPath
    Private NotificationText As String = Nothing
    Private _NotificationType As Type
    Private _RoundedCorners As Boolean
    Private _ShowCloseButton As Boolean
    Private _Image As Image
    Private _ImageSize As Size

#End Region
#Region " Enums "

    ' Create a list of Notification Types
    Enum Type
        [Notice]
        [Success]
        [Warning]
        [Error]
    End Enum

#End Region
#Region " Custom Properties "

    ' Create a NotificationType property and add the Type enum to it
    Public Property NotificationType As Type
        Get
            Return _NotificationType
        End Get
        Set(ByVal value As Type)
            _NotificationType = value
            Invalidate()
        End Set
    End Property
    ' Boolean value to determine whether the control should use border radius
    Public Property RoundCorners As Boolean
        Get
            Return _RoundedCorners
        End Get
        Set(ByVal value As Boolean)
            _RoundedCorners = value
            Invalidate()
        End Set
    End Property
    ' Boolean value to determine whether the control should draw the close button
    Public Property ShowCloseButton As Boolean
        Get
            Return _ShowCloseButton
        End Get
        Set(ByVal value As Boolean)
            _ShowCloseButton = value
            Invalidate()
        End Set
    End Property
    ' Integer value to determine the curve level of the borders
    Public Property BorderCurve As Integer
        Get
            Return _BorderCurve
        End Get
        Set(ByVal value As Integer)
            _BorderCurve = value
            Invalidate()
        End Set
    End Property
    ' Image value to determine whether the control should draw an image before the header
    Property Image() As Image
        Get
            Return _Image
        End Get
        Set(ByVal value As Image)
            If value Is Nothing Then
                _ImageSize = Size.Empty
            Else
                _ImageSize = value.Size
            End If

            _Image = value
            Invalidate()
        End Set
    End Property
    ' Size value - returns the image size
    Protected ReadOnly Property ImageSize() As Size
        Get
            Return _ImageSize
        End Get
    End Property

#End Region
#Region " EventArgs "

    Protected Overrides Sub OnMouseMove(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseMove(e)

        ' Decides the location of the drawn ellipse. If mouse is over the correct coordinates, "IsOverClose" boolean will be triggered to draw the ellipse
        If e.X >= Width - 19 AndAlso e.X <= Width - 10 AndAlso e.Y > CloseCoordinates.Y AndAlso e.Y < CloseCoordinates.Y + 12 Then
            IsOverClose = True
        Else
            IsOverClose = False
        End If
        ' Updates the control
        Invalidate()
    End Sub
    Protected Overrides Sub OnMouseDown(ByVal e As System.Windows.Forms.MouseEventArgs)
        MyBase.OnMouseDown(e)

        ' Disposes the control when the close button is clicked
        If _ShowCloseButton = True Then
            If IsOverClose Then
                Dispose()
            End If
        End If
    End Sub

#End Region

    Friend Function CreateRoundRect(ByVal r As Rectangle, ByVal curve As Integer) As GraphicsPath
        ' Draw a border radius
        Try
            CreateRoundPath = New GraphicsPath(FillMode.Winding)
            CreateRoundPath.AddArc(r.X, r.Y, curve, curve, 180.0F, 90.0F)
            CreateRoundPath.AddArc(r.Right - curve, r.Y, curve, curve, 270.0F, 90.0F)
            CreateRoundPath.AddArc(r.Right - curve, r.Bottom - curve, curve, curve, 0.0F, 90.0F)
            CreateRoundPath.AddArc(r.X, r.Bottom - curve, curve, curve, 90.0F, 90.0F)
            CreateRoundPath.CloseFigure()
        Catch ex As Exception
            MessageBox.Show(ex.Message & vbNewLine & vbNewLine & "Value must be either '1' or higher", "Invalid Integer", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            ' Return to the default border curve if the parameter is less than "1"
            _BorderCurve = 8
            BorderCurve = 8
        End Try
        Return CreateRoundPath
    End Function

    Sub New()
        SetStyle(ControlStyles.AllPaintingInWmPaint Or
                     ControlStyles.UserPaint Or
                     ControlStyles.OptimizedDoubleBuffer Or
                     ControlStyles.ResizeRedraw, True)

        Font = New Font("Tahoma", 9)
        Me.MinimumSize = New Size(100, 40)
        RoundCorners = False
        ShowCloseButton = True
    End Sub

    Protected Overrides Sub OnPaint(ByVal e As System.Windows.Forms.PaintEventArgs)
        MyBase.OnPaint(e)

        ' Declare Graphics to draw the control
        Dim GFX As Graphics = e.Graphics
        ' Declare Color to paint the control's Text, Background and Border
        Dim ForeColor, BackgroundColor, BorderColor As Color
        ' Determine the header Notification Type font
        Dim TypeFont As New Font(Font.FontFamily, Font.Size, FontStyle.Bold)
        ' Decalre a new rectangle to draw the control inside it
        Dim MainRectangle As New Rectangle(0, 0, Width - 1, Height - 1)
        ' Declare a GraphicsPath to create a border radius
        Dim CrvBorderPath As GraphicsPath = CreateRoundRect(MainRectangle, _BorderCurve)

        GFX.SmoothingMode = SmoothingMode.HighQuality
        GFX.TextRenderingHint = Drawing.Text.TextRenderingHint.ClearTypeGridFit
        GFX.Clear(Parent.BackColor)

        Select Case _NotificationType
            Case Type.Notice
                BackgroundColor = Color.FromArgb(111, 177, 199)
                BorderColor = Color.FromArgb(111, 177, 199)
                ForeColor = Color.White
            Case Type.Success
                BackgroundColor = Color.FromArgb(91, 195, 162)
                BorderColor = Color.FromArgb(91, 195, 162)
                ForeColor = Color.White
            Case Type.Warning
                BackgroundColor = Color.FromArgb(254, 209, 108)
                BorderColor = Color.FromArgb(254, 209, 108)
                ForeColor = Color.DimGray
            Case Type.Error
                BackgroundColor = Color.FromArgb(217, 103, 93)
                BorderColor = Color.FromArgb(217, 103, 93)
                ForeColor = Color.White
        End Select

        If _RoundedCorners = True Then
            GFX.FillPath(New SolidBrush(BackgroundColor), CrvBorderPath)
            GFX.DrawPath(New Pen(BorderColor), CrvBorderPath)
        Else
            GFX.FillRectangle(New SolidBrush(BackgroundColor), MainRectangle)
            GFX.DrawRectangle(New Pen(BorderColor), MainRectangle)
        End If

        Select Case _NotificationType
            Case Type.Notice
                NotificationText = "NOTICE"
            Case Type.Success
                NotificationText = "SUCCESS"
            Case Type.Warning
                NotificationText = "WARNING"
            Case Type.Error
                NotificationText = "ERROR"
        End Select

        If IsNothing(Image) Then
            GFX.DrawString(NotificationText, TypeFont, New SolidBrush(ForeColor), New Point(10, 5))
            GFX.DrawString(Text, Font, New SolidBrush(ForeColor), New Rectangle(10, 21, Width - 17, Height - 5))
        Else
            GFX.DrawImage(_Image, 12, 4, 16, 16)
            GFX.DrawString(NotificationText, TypeFont, New SolidBrush(ForeColor), New Point(30, 5))
            GFX.DrawString(Text, Font, New SolidBrush(ForeColor), New Rectangle(10, 21, Width - 17, Height - 5))
        End If

        CloseCoordinates = New Point(Width - 26, 4)

        If _ShowCloseButton = True Then
            ' Draw the close button
            GFX.DrawString("r", New Font("Marlett", 7, FontStyle.Regular), New SolidBrush(Color.FromArgb(130, 130, 130)), New Rectangle(Width - 20, 10, Width, Height), New StringFormat() With {.Alignment = StringAlignment.Near, .LineAlignment = StringAlignment.Near})
        End If

        CrvBorderPath.Dispose()
    End Sub
End Class

#End Region


Public Class UIStyleManager
    Inherits Component
    Public Sub New()
    End Sub



    Private val As Form
    Public Property Formulario As Form
        Get
            Return val
        End Get
        Set(value As Form)
            val = value
            CambiarColorFrm(value)
        End Set
    End Property

    Private Radio As Integer = 5
    Public Property ElipseRadius As Integer
        Get
            Return Radio
        End Get
        Set(value As Integer)
            Radio = value
        End Set
    End Property

    Public Sub New(container As Container)
        Me.New()
        container.Add(Me)
        Formulario.BackColor = Color.Red
        CambiarColorFrm(Formulario)
    End Sub
    <DllImport("Gdi32.dll", EntryPoint:="CreateRoundRectRgn")>
    Private Shared Function CreateRoundRectRgn(nLeftRect As Integer, nTopRect As Integer, nRightRect As Integer, nBottomRect As Integer, nWidthEllipse As Integer, nHeightEllipse As Integer) As IntPtr
    End Function
    Public Shared Sub CambiarColorFrm(frm As Form)
        'frm.BackColor = System.Drawing.Color.DarkBlue
        frm.FormBorderStyle = FormBorderStyle.None
        'frm = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 5, 5))

    End Sub
End Class