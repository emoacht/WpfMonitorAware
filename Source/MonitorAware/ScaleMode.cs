
namespace MonitorAware
{
	/// <summary>
	/// Scaling mode of handler for <see cref="System.Windows.Window"/>
	/// </summary>
	public enum ScaleMode
	{
		/// <summary>
		/// Forbear scaling and leave it to the built-in functionality.
		/// </summary>
		Forbear = 0,

		/// <summary>
		/// Invoke scaling while target Window is still moving.
		/// </summary>
		InvokeWhileMoving,

		/// <summary>
		/// Resolve scaling after target Window has moved.
		/// </summary>
		ResolveAfterMoved
	}
}