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

#Installation

Try to download or fork the source code and compile it.  
Or: You can download the latest version via NuGet: [https://nuget.org/packages/gong-wpf-dragdrop](https://nuget.org/packages/gong-wpf-dragdrop/)

#Release History

+ **0.1.3.9** (11 Aug, 2013)
	- minor change to default insertion adorner (bigger triangle size)
	- nuget package for NET 3.5 should be working (DataGrid)
	- Issue [#62](https://github.com/punker76/gong-wpf-dragdrop/issues/62): Now we can resize GridView and DataGrid columns.
	- Issue [#25](https://github.com/punker76/gong-wpf-dragdrop/issues/25): Don't allow dragging item onto scroll bars.
	- Issue [#30](https://github.com/punker76/gong-wpf-dragdrop/issues/30): Add support for derived item container classes.
	- Issue [#69](https://github.com/punker76/gong-wpf-dragdrop/issues/69): Don't clip Drag Adorner.
	- Issue [#33](https://github.com/punker76/gong-wpf-dragdrop/issues/33): Dragging a single item from a grouped datagrid will often return CollectionViewGroupInternal rather than the item.
	- Issue [#70](https://github.com/punker76/gong-wpf-dragdrop/issues/70): Fix Drag&Drop to empty DataGrid.
	- Issue [#14](https://github.com/punker76/gong-wpf-dragdrop/issues/14): Raise DropCompleted / DragCompleted
+ **0.1.3.8** (20 July, 2013)
	- Issue [#38](https://github.com/punker76/gong-wpf-dragdrop/issues/38):
		- add DragMouseAnchorPoint dependency property for drag adorner positioning
		- better capture screen for default drag adorner
	- Issue [#42](https://github.com/punker76/gong-wpf-dragdrop/issues/42): Now we can have items deriving from ContentControl.
+ **0.1.3.7** (5 June, 2013)
	- Issue [#57](https://github.com/punker76/gong-wpf-dragdrop/issues/57): Remove opacity modification of adorned item (will be used only for default adorned items)
	- Fix possible exception with drag&drop from outside the window or something else
	- Issue [#49](https://github.com/punker76/gong-wpf-dragdrop/issues/49): The Dropped function from IDragSource is called on the target and not the Source
	- Issue [#37](https://github.com/punker76/gong-wpf-dragdrop/issues/37), Issue [#12](https://github.com/punker76/gong-wpf-dragdrop/issues/12):
		- Slider in list item can't be changed. DD thinks the list item is being dragged.
		- Text box on a list item: when I try to select the text by doing a drag selection the list item starts to be dragged.
	- Issue [#63](https://github.com/punker76/gong-wpf-dragdrop/issues/63): Multiple items deselected in ListView and other subclasses of ListBox. thx to [rdingwall](https://github.com/rdingwall) (Richard Dingwall)
		- Fixed over-strict IsAssignableFrom, was causing bug where ListViews and other subclasses of ListBox would lose their selections if multiple items were selected.
	- Issue [#15](https://github.com/punker76/gong-wpf-dragdrop/issues/15): A drop target can now be placed on a child element of a grid, listbox or something else (more mvvm like). The NotHandled property can be used to allow bubbling the drag over event.
	- Issue [#64](https://github.com/punker76/gong-wpf-dragdrop/issues/64): Now it's possible to use ItemsControl itself as drag source and drop target.
	- Issue [#59](https://github.com/punker76/gong-wpf-dragdrop/issues/59): Fix Exception when using WPF UserControl under ElementHost in an Excel AddIn. If no window is found, search again for a UserControl.
+ **0.1.3.6** (25 March, 2013)
	- add changes from mitchel.jon (latest google code) branch
		- Made changes to make the drag drop look more like the Windows 7 file explorer drag drop.
		- Displays the drag adorner all the time even when the DragDropEffect is None.
		- Offset the adorner so that the cursor is sat at the bottom middle of the adornment.
		- Added a DragDropEffect adorner object to allow the addition of a WPF adornment for feedback instead of the standard monochrome cursor.
		- Added dependency properties to allow the user to define their own DragDropEffect adorners.
		- Added default DragDropEffect adorners for the effect None/Copy/Move/Link.
		- Changed the events to tunnel instead of bubble. - Improved drag effect adorner.
		- Removed flicker from drag adorner.
		- Added support for controls that inherit from TreeView. (might work with other derived controls, but not tested)
		- Added drop destination text to DropInfo for the effect adorner to display.
		- Fixed issue with cursor override not being reset on drop.
		- A few updates to improve drag and drop for TreeViews.
		- Added RelativeInsertPosition.TargetItemCenter enum so that the DropInfo can tell the end user that the drop area is the middle 50% of the target item.
		- Marked assembly as CLSCompliant.
		- Add the cursor position relative to the dragged item when stating a drag operation to the DragInfo object. The allows me to position the default drag adorner under the cursor correctly.
		- Added the ability to explicitly ignore drag and drop for a particular control in visual tree. e.g. If you have a TextBox in a list control that supports drag and drop, you can't do a drag selection of the text in the TextBox. If you explicitly ignore it then drag selection will work correctly.
		- Fixed cast exception when attempting to start a drag operation from a FrameworkContentElement. Need to use its parent as it can't be cast as a UIElement.

	- new property UseDefaultDragAdorner: if no drag adorner template is given use a simple default adorner (default is false)
	- new property UseDefaultEffectDataTemplate: if no effect data template is given use a default template (default is false)
	- fix size of default drag adorner

	- Issue [#38](https://github.com/punker76/gong-wpf-dragdrop/issues/38): display visual of dragged item instead of using template
	- Issue [#39](https://github.com/punker76/gong-wpf-dragdrop/issues/39): Highlight adorner outlines too much when TreeView item is collapsed
	- Issue [#55](https://github.com/punker76/gong-wpf-dragdrop/issues/55): Wrong behaviour when listbox panel is StackPanel FlowDirection="RightToLeft" Orientation="Horizontal"
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

```xml
<ListBox ItemsSource="{Binding Collection}"
         dd:DragDrop.IsDragSource="True"
         dd:DragDrop.IsDropTarget="True" />
```

Setting the IsDragSource and IsDropTarget attached propeties to True on an ItemsControl such as ListBox enables drag and drop. The default behaviour is to allow re-ordering of items within the control.

If your project contains another ItemsControl with drag/drop enabled in this manner, and it is bound to a collection of the same type, then items can also be dragged and dropped between the controls.

###Adding a Drop Handler

While the defaults can be useful in simple cases, you will usually want more control of what happens when data is dragged/dropped onto your control. You can delegate that responsibility to your ViewModel by setting the DropHandler attached property:

```xml
<ListBox ItemsSource="{Binding Collection}"
         dd:DragDrop.IsDragSource="True"
         dd:DragDrop.IsDropTarget="True"
         dd:DragDrop.DropHandler="{Binding}" />
```

In this example, we're binding the drop handler to the current DataContext, which will usually be your ViewModel.

You handle the drop in your ViewModel by implementing the IDropTarget interface:

```csharp
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
