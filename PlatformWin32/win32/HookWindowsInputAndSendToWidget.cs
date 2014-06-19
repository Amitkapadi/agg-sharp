﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MatterHackers.Agg.PlatfromAbstract;

namespace MatterHackers.Agg.UI
{
    public class HookWindowsInputAndSendToWidget
    {
        protected WidgetForWindowsFormsAbstract widgetToSendTo;

        public HookWindowsInputAndSendToWidget(ContainerControl controlToHook, WidgetForWindowsFormsAbstract widgetToSendTo)
        {
            this.widgetToSendTo = widgetToSendTo;
            controlToHook.GotFocus += new EventHandler(controlToHook_GotFocus);
            controlToHook.LostFocus += new EventHandler(controlToHook_LostFocus);

            controlToHook.KeyDown += new System.Windows.Forms.KeyEventHandler(controlToHook_KeyDown);
            controlToHook.KeyUp += new System.Windows.Forms.KeyEventHandler(controlToHook_KeyUp);
            controlToHook.KeyPress += new System.Windows.Forms.KeyPressEventHandler(controlToHook_KeyPress);

            controlToHook.MouseDown += new MouseEventHandler(controlToHook_MouseDown);
            controlToHook.MouseMove += new MouseEventHandler(formToHook_MouseMove);
            controlToHook.MouseUp += new MouseEventHandler(controlToHook_MouseUp);
            controlToHook.MouseWheel += new MouseEventHandler(controlToHook_MouseWheel);

            controlToHook.MouseCaptureChanged += new EventHandler(controlToHook_MouseCaptureChanged);

            controlToHook.MouseLeave += new EventHandler(controlToHook_MouseLeave);
        }

        void controlToHook_MouseLeave(object sender, EventArgs e)
        {
            widgetToSendTo.OnMouseMove(new MatterHackers.Agg.UI.MouseEventArgs(MatterHackers.Agg.UI.MouseButtons.None, 0, -10, -10, 0));
        }

        void controlToHook_GotFocus(object sender, EventArgs e)
        {
            widgetToSendTo.OnGotFocus(e);
        }

        void controlToHook_LostFocus(object sender, EventArgs e)
        {
            widgetToSendTo.Unfocus();
            widgetToSendTo.OnLostFocus(e);
        }

        void controlToHook_KeyDown(object sender, System.Windows.Forms.KeyEventArgs windowsKeyEvent)
        {
			MatterHackers.Agg.UI.KeyEventArgs aggKeyEvent;
			if (OsInformation.GetOSType () == OsInformation.OSType.Mac 
				&& (windowsKeyEvent.KeyData & System.Windows.Forms.Keys.Alt) == System.Windows.Forms.Keys.Alt) 
			{

				aggKeyEvent = new MatterHackers.Agg.UI.KeyEventArgs ((MatterHackers.Agg.UI.Keys)(System.Windows.Forms.Keys.Control | (windowsKeyEvent.KeyData & ~System.Windows.Forms.Keys.Alt)));
			}
			else 
			{
				aggKeyEvent = new MatterHackers.Agg.UI.KeyEventArgs ((MatterHackers.Agg.UI.Keys)windowsKeyEvent.KeyData);
			}
            widgetToSendTo.OnKeyDown(aggKeyEvent);

            Keyboard.SetKeyDownState(aggKeyEvent.KeyCode, true);

            windowsKeyEvent.Handled = aggKeyEvent.Handled;
            windowsKeyEvent.SuppressKeyPress = aggKeyEvent.SuppressKeyPress;
        }

        void controlToHook_KeyUp(object sender, System.Windows.Forms.KeyEventArgs windowsKeyEvent)
        {
            MatterHackers.Agg.UI.KeyEventArgs aggKeyEvent = new MatterHackers.Agg.UI.KeyEventArgs((MatterHackers.Agg.UI.Keys)windowsKeyEvent.KeyData);
            widgetToSendTo.OnKeyUp(aggKeyEvent);

            Keyboard.SetKeyDownState(aggKeyEvent.KeyCode, false);

            windowsKeyEvent.Handled = aggKeyEvent.Handled;
            windowsKeyEvent.SuppressKeyPress = aggKeyEvent.SuppressKeyPress;
        }

        void controlToHook_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs windowsKeyPressEvent)
        {
            MatterHackers.Agg.UI.KeyPressEventArgs aggKeyPressEvent = new MatterHackers.Agg.UI.KeyPressEventArgs(windowsKeyPressEvent.KeyChar);
            widgetToSendTo.OnKeyPress(aggKeyPressEvent);
            windowsKeyPressEvent.Handled = aggKeyPressEvent.Handled;
        }

        MatterHackers.Agg.UI.MouseEventArgs ConvertWindowsMouseEventToAggMouseEvent(System.Windows.Forms.MouseEventArgs windowsMouseEvent)
        {
            // we invert the y as we are bottom left coordinate system and windows is top left.
            int Y = windowsMouseEvent.Y;
            Y = (int)widgetToSendTo.height() - Y;

            return new MatterHackers.Agg.UI.MouseEventArgs((MatterHackers.Agg.UI.MouseButtons)windowsMouseEvent.Button, windowsMouseEvent.Clicks, windowsMouseEvent.X, Y, windowsMouseEvent.Delta);
        }

        void controlToHook_MouseDown(object sender, System.Windows.Forms.MouseEventArgs windowsMouseEvent)
        {
            widgetToSendTo.OnMouseDown(ConvertWindowsMouseEventToAggMouseEvent(windowsMouseEvent));
        }

        void formToHook_MouseMove(object sender, System.Windows.Forms.MouseEventArgs windowsMouseEvent)
        {
            widgetToSendTo.OnMouseMove(ConvertWindowsMouseEventToAggMouseEvent(windowsMouseEvent));
        }

        void controlToHook_MouseUp(object sender, System.Windows.Forms.MouseEventArgs windowsMouseEvent)
        {
            widgetToSendTo.OnMouseUp(ConvertWindowsMouseEventToAggMouseEvent(windowsMouseEvent));
        }

        void controlToHook_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (widgetToSendTo.ChildHasMouseCaptured || widgetToSendTo.MouseCaptured)
            {
                widgetToSendTo.OnMouseUp(new MatterHackers.Agg.UI.MouseEventArgs(MouseButtons.Left, 0, -10, -10, 0));
            }
        }

        void controlToHook_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs windowsMouseEvent)
        {
            widgetToSendTo.OnMouseWheel(ConvertWindowsMouseEventToAggMouseEvent(windowsMouseEvent));
        }
    }
}
