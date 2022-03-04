using System;
using Eto.Forms;

namespace Eto.GtkSharp.Forms.Controls
{
	public class ProgressBarHandler : GtkControl<Gtk.ProgressBar, ProgressBar, ProgressBar.ICallback>, ProgressBar.IHandler
	{
		double minValue;
		double maxValue = 100;
		bool indeterminate;
		UITimer timer;
		public static double UpdateInterval = 0.2;
		public static double PulseStep = 0.1;

		public ProgressBarHandler()
		{
			this.Control = new Gtk.ProgressBar
			{
				Fraction = 0
			};
		}

		protected new ProgressBarConnector Connector { get { return (ProgressBarConnector)base.Connector; } }

		protected override WeakConnector CreateConnector()
		{
			return new ProgressBarConnector();
		}

		protected class ProgressBarConnector : GtkControlConnector
		{
			public new ProgressBarHandler Handler { get { return (ProgressBarHandler)base.Handler; } }

			public void TimerElapsed(object sender, EventArgs e)
			{
				var timer = (UITimer)sender;
				var handler = Handler;
				if (handler != null)
					handler.Control.Pulse();
				else
					timer.Stop();
			}
		}

		public bool Indeterminate
		{
			get { return indeterminate; }
			set
			{
				indeterminate = value;
				if (indeterminate)
				{
					if (timer == null)
					{
						timer = new UITimer();
						timer.Elapsed += Connector.TimerElapsed;
					}
					timer.Interval = UpdateInterval;
					Control.PulseStep = PulseStep;
					timer.Start();
				}
				else if (timer != null)
					timer.Stop();
			}
		}

		public double MaxValue
		{
			get { return maxValue; }
			set
			{
				var val = Value;
				maxValue = value;
				Value = val;
			}
		}

		public double MinValue
		{
			get { return minValue; }
			set
			{
				var val = Value;
				minValue = value;
				Value = val;
			}
		}

		public double Value
		{
			get { return Control.Fraction * MaxValue + MinValue; }
			set
			{
				Control.Fraction = Math.Max(0, Math.Min(1, (value - MinValue) / MaxValue));
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (timer != null)
				timer.Stop();
		}
	}
}

