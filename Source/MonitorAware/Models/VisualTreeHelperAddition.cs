using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MonitorAware.Models
{
	/// <summary>
	/// Additional methods for <see cref="System.Windows.Media.VisualTreeHelper"/>
	/// </summary>
	public class VisualTreeHelperAddition
	{
		/// <summary>
		/// Gets the first descendant visual of a specified visual.
		/// </summary>
		/// <typeparam name="T">Type of descendant visual</typeparam>
		/// <param name="reference">Ancestor visual</param>
		/// <returns>Descendant visual if successfully gets. Null otherwise.</returns>
		public static T GetDescendant<T>(DependencyObject reference) where T : DependencyObject
		{
			return TryGetDescendant<T>(reference, out T descendent) ? descendent : null;
		}

		/// <summary>
		/// Attempts to get the first descendant visual of a specified visual.
		/// </summary>
		/// <typeparam name="T">Type of descendant visual</typeparam>
		/// <param name="reference">Ancestor visual</param>
		/// <param name="descendant">Descendant visual</param>
		/// <returns>True if successfully gets. False otherwise.</returns>
		public static bool TryGetDescendant<T>(DependencyObject reference, out T descendant) where T : DependencyObject
		{
			var queue = new Queue<DependencyObject>();
			var parent = reference;

			while (parent != null)
			{
				var count = VisualTreeHelper.GetChildrenCount(parent);
				for (int i = 0; i < count; i++)
				{
					var child = VisualTreeHelper.GetChild(parent, i);
					if (child is T buffer)
					{
						descendant = buffer;
						return true;
					}
					queue.Enqueue(child);
				}

				parent = (0 < queue.Count) ? queue.Dequeue() : null;
			}

			descendant = default;
			return false;
		}
	}
}