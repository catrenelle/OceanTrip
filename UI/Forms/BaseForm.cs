using Ocean_Trip.Properties;
using System;
using System.Windows.Forms;

namespace Ocean_Trip
{
	/// <summary>
	/// Base form providing common functionality for all child forms in the settings UI
	/// </summary>
	public class BaseForm : Form
	{
		protected new FormSettings ParentForm;
		protected PictureBox ExitIcon;

		/// <summary>
		/// Initialize the base form with parent reference and optional exit icon
		/// </summary>
		/// <param name="parent">Parent FormSettings instance</param>
		/// <param name="exitIcon">PictureBox control to use as exit button (optional)</param>
		protected void InitializeBaseForm(FormSettings parent, PictureBox exitIcon = null)
		{
			ParentForm = parent;

			if (exitIcon != null)
			{
				ExitIcon = exitIcon;
				SetupExitIcon();
			}
		}

		/// <summary>
		/// Setup exit icon with grayscale default, hover effects, and click handler
		/// </summary>
		private void SetupExitIcon()
		{
			if (ExitIcon == null)
				return;

			// Set initial grayscale image
			ExitIcon.Image = ImageExtensions.ToGrayScale(Resources.exit);

			// Wire up event handlers
			ExitIcon.Click += ExitIcon_Click;
			ExitIcon.MouseEnter += ExitIcon_MouseEnter;
			ExitIcon.MouseLeave += ExitIcon_MouseLeave;
		}

		/// <summary>
		/// Handle exit icon click - closes parent form
		/// </summary>
		private void ExitIcon_Click(object sender, EventArgs e)
		{
			ParentForm?.Close();
		}

		/// <summary>
		/// Handle mouse enter - show colored exit icon
		/// </summary>
		private void ExitIcon_MouseEnter(object sender, EventArgs e)
		{
			if (ExitIcon != null)
				ExitIcon.Image = Resources.exit;
		}

		/// <summary>
		/// Handle mouse leave - show grayscale exit icon
		/// </summary>
		private void ExitIcon_MouseLeave(object sender, EventArgs e)
		{
			if (ExitIcon != null)
				ExitIcon.Image = ImageExtensions.ToGrayScale(Resources.exit);
		}

		/// <summary>
		/// Handle mouse down for window dragging
		/// Call this from child form's MouseDown event
		/// </summary>
		protected void HandleMouseDown(object sender, MouseEventArgs e)
		{
			ParentForm?.MoveWindow(sender, e);
		}

		/// <summary>
		/// Handle keyboard shortcuts (e.g., Escape to close)
		/// Call this from child form's KeyUp event
		/// </summary>
		protected void HandleKeyUp(object sender, KeyEventArgs e)
		{
			ParentForm?.FormSettings_KeyUp(sender, e);
		}
	}
}
