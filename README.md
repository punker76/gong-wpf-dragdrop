#Introduction

The GongSolutions.Wpf.DragDrop library is a drag'n'drop framework for WPF.

It has the following features:

+ Works with MVVM : the logic for the drag and drop can be placed in a ViewModel. No code needs to be placed in codebehind, instead attached properties are used to bind to a drag handler/drop handler in a ViewModel.
+ Works with multiple selections.
+ Can drag data within the same control to re-order, or between controls.
+ Works with TreeViews.
+ Can insert an item into a collection, or drop one item onto another.
+ Can display Adorners to give the user visual feedback of the operation in progress.
+ Has sensible defaults so that you have to write less code for common operations.

#Current Status

The library is in its early stages, but should still be useful and reasonably bug-free.

#Installation

You can download latest version via NuGet: [https://nuget.org/packages/gong-wpf-dragdrop](https://nuget.org/packages/gong-wpf-dragdrop/)

#Release History

+ **0.1.3.5** (17 March, 2013)
	- Issue #17: 'System.Windows.Documents.Run' is not a Visual or Visual3D' Exception
	- Pull request #52: Sometimes ItemsControlExtensions throws an ArgumentOutOfRangeException
	- Pull request #53: Add support for custom IDataObject implementations
	- Issue #22: Problem with drag n drop when ContextMenu is open
	- .NET 3.5 and .NET 4 nuget target
+ **0.1.3.4** (18 Feb, 2013)
	- Issue #45: After doublclick, item loose focus. No drag drop stuff with multiple mouse clicks!
	- Issue #46: Added a KeyStates property to the DropInfo class.
	- Issue #40 make DefaultDropHandler and some methods public
+ **0.1.3.3** (16 Feb, 2013)
	- Fix Issue [34](http://code.google.com/p/gong-wpf-dragdrop/issues/detail?id=34): Error with HitTestScrollBar thx to [rdingwall](https://github.com/rdingwall) (Richard Dingwall)
	- Fix for possible exception: Unable to cast object of type 'System.Windows.Documents.Run' to type 'System.Windows.UIElement'. thx to [rdingwall](https://github.com/rdingwall) (Richard Dingwall)
+ **0.1.3.2** (15 Feb, 2013)
	- add icon url and license url for nuget package
	- add Steven Kirk to authors and owners
+ **0.1.3.1** (7 Feb, 2013)
	- Fix possible null pointer exception for getting the item parent at DragInfo. 
	- Create NuGet package.
+ **0.1.3** (July, 2010)
	- Added a Dropped method to IDragSource which is called to notify the drag handler that a drop has been performed.
	- Issue #24. Applied patch from aamillsbit to use interfaces for Drag and Drop Info. Makes it easier to unit test handlers.
	- Issue #20: Deselect selected ListViewItem
	Make selection in MouseLeftButtonUp work when Ctrl key is pressed.
	- Issue #19: DropPosition
	Add DropPosition as public property of DropInfo.
	- Issue #21: Select single item (ListViewItem) from current selection.
	Set SelectedItem to null before setting it to the new value. Do this because if the new value was the first item already selected in the control, nothing would happen.
	- Fix Issue #9: Drag ListBoxItem that includes a TextBox. When a TextBox is included in a list box's data template, System.Windows.DragDrop.DoDragDrop() ends up being called twice, which results in m_DragInfo being null when the second call to the method exits. Make sure that DoDragDrop is only called once.
	- Fix for issue #11: Patch from sportfrank which fixes adorners not being shown on windows other than the main window.
	- Fix for Issue #8: DragSource with extended selection. Make clicking on an already selected item select only that item on mouse up. Added additional check for null m_DragInfo to handle case where drag is cancelled.
	- Made changes to make the drag drop look more like the Windows 7 file explorer drag drop.
		- Displays the drag adorner all the time even when the DragDropEffect is None.
		- Offset the adorner so that the cursor is sat at the bottom middle of the adornment.
		- Added a DragDropEffect adorner object to allow the addition of a WPF adornment for feedback instead of the standard monochrome cursor.
		- Added dependency properties to allow the user to define their own DragDropEffect adorners.
		- Added default DragDropEffect adorners for the effect None/Copy/Move/Link.
+ **0.1.2** (20 Dec, 2009)
	- Added support for groups within ItemsControls.
	- Use a better algorithm to determine the InsertPosition of a drop.
+ **0.1.1** (25 Nov, 2009)
	- Fixed a bug where mouse clicks were being unnecessarily swallowed on multi-selectable ItemsControls.
	- Documented most commonly used classes.
+ **0.1.0** (11 Nov, 2009)
	- Initial Release.

#Examples

###Default Behaviour

A simple example of adding drag/drop to a ListBox:

```
<ListBox ItemsSource="{Binding Collection}"
         dd:DragDrop.IsDragSource="True"
         dd:DragDrop.IsDropTarget="True" />
```

Setting the IsDragSource and IsDropTarget attached propeties to True on an ItemsControl such as ListBox enables drag and drop. The default behaviour is to allow re-ordering of items within the control.

If your project contains another ItemsControl with drag/drop enabled in this manner, and it is bound to a collection of the same type, then items can also be dragged and dropped between the controls.

###Adding a Drop Handler

While the defaults can be useful in simple cases, you will usually want more control of what happens when data is dragged/dropped onto your control. You can delegate that responsibility to your ViewModel by setting the DropHandler attached property:

```
<ListBox ItemsSource="{Binding Collection}"
         dd:DragDrop.IsDragSource="True"
         dd:DragDrop.IsDropTarget="True"
         dd:DragDrop.DropHandler="{Binding}" />
```

In this example, we're binding the drop handler to the current DataContext, which will usually be your ViewModel.

You handle the drop in your ViewModel by implementing the IDropTarget interface:

```
class ExampleViewModel : IDropTarget
{
	public ObservableCollection<ExampleItemViewModel> Items;
	
	void IDropTarget.DragOver(IDropInfo dropInfo) {
		ExampleItemViewModel sourceItem = dropInfo.Data as ExampleItemViewModel;
		ExampleItemViewModel targetItem = dropInfo.TargetItem as ExampleItemViewModel;
		
		if (sourceItem != null && targetItem != null && targetItem.CanAcceptChildren) {
			dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
			dropInfo.Effects = DragDropEffects.Copy;
		}
	}
	
	void IDropTarget.Drop(IDropInfo dropInfo) {
		ExampleItemViewModel sourceItem = dropInfo.Data as ExampleItemViewModel;
		ExampleItemViewModel targetItem = dropInfo.TargetItem as ExampleItemViewModel;
		targetItem.Children.Add(sourceItem);
	}
}

class ExampleItemViewModel
{
	public bool CanAcceptChildren { get; set; }
	public ObservableCollection<ExampleItemViewModel> Children { get; private set; }
}
```

In this example, we're checking that the item being dragged and the item being dropped onto are both ExampleItemViewModels and that the target item allows items to be added to its Children collection. If the drag satisfies both of these conditions, then the function tells the framework to display a Copy mouse pointer, and to use a Highlight drop target adorner.

For more information, check out the full [DropHandlerExample](Examples).

#License

[New BSD License](http://opensource.org/licenses/BSD-3-Clause)
