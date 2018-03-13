using Microsoft.Xaml.Interactivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Nameday
{
    [TypeConstraint(typeof(ListViewBase))]
    public class AutoScrollSelectedItemBehavior : DependencyObject, IBehavior
    {
        private ListViewBase AssociatedListView => AssociatedObject as ListView;

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;

            if (AssociatedListView != null)
                AssociatedListView.SelectionChanged += ListViewOnSelectionChanged;
        }

        public void Detach()
        {
            if (AssociatedListView != null)
                AssociatedListView.SelectionChanged -= ListViewOnSelectionChanged;
        }

        private void ListViewOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AssociatedListView.SelectedItem == null)
                return;
            
            AssociatedListView.ScrollIntoView(AssociatedListView.SelectedItem);
        }
    }
}
