using System.Drawing;

namespace Profit_Order.Controls
{
    public class PanZoom
    {
        bool pitching = false;
        bool panning = false;

        bool collectFirst = false;

        double xOffset = 0;
        double yOffset = 0;

        //scale processing...
        double scaleMin;
        double scaleMax;
        double scale;

        double _xScaleOrigin;
        double _yScaleOrigin;

        double panTotalX;
        double panTotalY;

        ContentPage contentPage;
        View Content;
        public void Setup(ContentPage cp, View content)
        {
            contentPage = cp;
            Content = content;

            PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchUpdated;
            contentPage.Content.GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            contentPage.Content.GestureRecognizers.Add(panGesture);

            contentPage.SizeChanged += (sender, e) => { layoutElements(); };
        }

        public void layoutElements()
        {
            if (contentPage.Width <= 0 || contentPage.Height <= 0 || Content.WidthRequest <= 0 || Content.HeightRequest <= 0)
                return;

            xOffset = 0;
            yOffset = 0;

            double pageW = contentPage.Width;
            double pageH = contentPage.Height;

            double w_s = pageW / Content.WidthRequest;
            double h_s = pageH / Content.HeightRequest;
            if (w_s < h_s)
                scaleMin = w_s;
            else
                scaleMin = h_s;
            scaleMax = scaleMin * 3.0;

            scale = scaleMin;

            double w = Content.WidthRequest * scale;
            double h = Content.HeightRequest * scale;
            double x = pageW / 2.0 - w / 2.0 + xOffset;
            double y = pageH / 2.0 - h / 2.0 + yOffset;

            AbsoluteLayout.SetLayoutBounds(Content, new Rect(x, y, w, h));
        }

        void fixPosition(
            ref double x, ref double y, ref double w, ref double h,
            bool setoffset
            )
        {
            double pageW = contentPage.Width;
            double pageH = contentPage.Height;


            if (w <= pageW)
            {
                double new_x = pageW / 2.0 - w / 2.0;
                if (setoffset)
                    xOffset = new_x - (pageW / 2.0 - w / 2.0);
                x = new_x;
            }
            else
            {
                if (x > 0)
                {
                    double new_x = 0;
                    if (setoffset)
                        xOffset = new_x - (pageW / 2.0 - w / 2.0);
                    x = new_x;
                }
                if (x < (pageW - w))
                {
                    double new_x = (pageW - w);
                    if (setoffset)
                        xOffset = new_x - (pageW / 2.0 - w / 2.0);
                    x = new_x;
                }
            }

            if (h <= pageH)
            {
                double new_y = pageH / 2.0 - h / 2.0;
                if (setoffset)
                    yOffset = new_y - (pageH / 2.0 - h / 2.0);
                y = new_y;
            }
            else
            {
                if (y > 0)
                {
                    double new_y = 0;
                    if (setoffset)
                        yOffset = new_y - (pageH / 2.0 - h / 2.0);
                    y = new_y;
                }
                if (y < (pageH - h))
                {
                    double new_y = (pageH - h);
                    if (setoffset)
                        yOffset = new_y - (pageH / 2.0 - h / 2.0);
                    y = new_y;
                }
            }
        }

        private void PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (sender != contentPage.Content)
                return;

            switch (e.Status)
            {
                case GestureStatus.Started:
                    {
                        pitching = true;
                        collectFirst = true;

                        double pageW = contentPage.Width;
                        double pageH = contentPage.Height;

                        _xScaleOrigin = e.ScaleOrigin.X * pageW;
                        _yScaleOrigin = e.ScaleOrigin.Y * pageH;
                    }
                    break;
                case GestureStatus.Running:
                    if (pitching)
                    {
                        double targetScale = scale * e.Scale;
                        targetScale = Math.Min(Math.Max(scaleMin, targetScale), scaleMax);

                        double scaleDelta = targetScale / scale;

                        double pageW = contentPage.Width;
                        double pageH = contentPage.Height;

                        double w_old = Content.WidthRequest * scale;
                        double h_old = Content.HeightRequest * scale;
                        double x_old = pageW / 2.0 - w_old / 2.0 + xOffset;
                        double y_old = pageH / 2.0 - h_old / 2.0 + yOffset;

                        scale = targetScale;

                        //new w and h
                        double w = Content.WidthRequest * scale;
                        double h = Content.HeightRequest * scale;

                        //transform x old and y old 
                        //   to get new scaled position over a pivot
                        double _x = (x_old - _xScaleOrigin) * scaleDelta + _xScaleOrigin;
                        double _y = (y_old - _yScaleOrigin) * scaleDelta + _yScaleOrigin;

                        //fix offset to be equal to _x and _y
                        double x = pageW / 2.0 - w / 2.0 + xOffset;
                        double y = pageH / 2.0 - h / 2.0 + yOffset;
                        xOffset += _x - x;
                        yOffset += _y - y;
                        x = pageW / 2.0 - w / 2.0 + xOffset;
                        y = pageH / 2.0 - h / 2.0 + yOffset;

                        fixPosition(ref x, ref y, ref w, ref h, true);

                        AbsoluteLayout.SetLayoutBounds(Content, new Rect(x, y, w, h));
                    }
                    break;
                case GestureStatus.Completed:
                    pitching = false;
                    break;
            }
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            if (sender != contentPage.Content)
                return;

            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    {
                        panning = true;
                        panTotalX = e.TotalX;
                        panTotalY = e.TotalY;
                        collectFirst = true;
                    }
                    break;
                case GestureStatus.Running:
                    if (panning)
                    {
                        if (collectFirst)
                        {
                            collectFirst = false;
                            panTotalX = e.TotalX;
                            panTotalY = e.TotalY;
                        }

                        double pageW = contentPage.Width;
                        double pageH = contentPage.Height;

                        double deltaX = e.TotalX - panTotalX;
                        double deltaY = e.TotalY - panTotalY;

                        panTotalX = e.TotalX;
                        panTotalY = e.TotalY;

                        xOffset += deltaX;
                        yOffset += deltaY;

                        double w = Content.WidthRequest * scale;
                        double h = Content.HeightRequest * scale;
                        double x = pageW / 2.0 - w / 2.0 + xOffset;
                        double y = pageH / 2.0 - h / 2.0 + yOffset;

                        fixPosition(ref x, ref y, ref w, ref h, true);

                        AbsoluteLayout.SetLayoutBounds(Content, new Rect(x, y, w, h));
                    }
                    break;
                case GestureStatus.Completed:
                    panning = false;
                    break;
            }
        }
    }
}
