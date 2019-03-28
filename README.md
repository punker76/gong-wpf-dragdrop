<div align="center">

  <a href="https://github.com/punker76/gong-wpf-dragdrop">
    <img alt="gong-wpf-dragdrop" width="700" heigth="142" src="./GongSolutions.Wpf.DragDrop.Full.png">
  </a>
  <h1>GongSolutions.WPF.DragDrop</h1>
  <p>
    An easy to use drag'n'drop framework for WPF.
  </p>

  <a href="https://gitter.im/punker76/gong-wpf-dragdrop">
	  <img src="https://img.shields.io/badge/Gitter-Join%20Chat-green.svg?style=flat-square">
  </a>
  <a href="https://twitter.com/punker76">
	  <img src="https://img.shields.io/badge/twitter-%40punker76-55acee.svg?style=flat-square">
  </a>
  <a href="https://ci.appveyor.com/project/punker76/gong-wpf-dragdrop/branch/master">
	  <img alt="masterstatus" src="https://img.shields.io/appveyor/ci/punker76/gong-wpf-dragdrop/master.svg?style=flat-square&&label=master">
  </a>
  <a href="https://ci.appveyor.com/project/punker76/gong-wpf-dragdrop/branch/dev">
	  <img alt="devstatus" src="https://img.shields.io/appveyor/ci/punker76/gong-wpf-dragdrop/dev.svg?style=flat-square&&label=dev">
  </a>
  <a href="https://github.com/punker76/gong-wpf-dragdrop/releases/latest">
	  <img src="https://img.shields.io/github/release/punker76/gong-wpf-dragdrop.svg?style=flat-square">
  </a>
  <br />
  <a href="https://www.nuget.org/packages/gong-wpf-dragdrop">
    <img src="https://img.shields.io/nuget/dt/gong-wpf-dragdrop.svg?style=flat-square">
  </a>
  <a href="https://www.nuget.org/packages/gong-wpf-dragdrop">
    <img src="https://img.shields.io/nuget/v/gong-wpf-dragdrop.svg?style=flat-square">
  </a>
  <a href="https://www.nuget.org/packages/gong-wpf-dragdrop">
    <img src="https://img.shields.io/nuget/vpre/gong-wpf-dragdrop.svg?style=flat-square&label=nuget-pre">
  </a>
  <a href="https://github.com/punker76/gong-wpf-dragdrop/issues">
    <img src="https://img.shields.io/github/issues/punker76/gong-wpf-dragdrop.svg?style=flat-square">
  </a>
  <br />
  <br />

</div>

## Features

+ Works with MVVM : the logic for the drag and drop can be placed in a ViewModel. No code needs to be placed in codebehind, instead attached properties are used to bind to a drag handler/drop handler in a ViewModel.
+ Works with multiple selections.
+ Can drag data within the same control to re-order, or between controls.
+ Works with `ListBox`, `ListView`, `TreeView`, `DataGrid` and any other `ItemsControl`.
+ Can insert, move or copy an item into a collection of the same control or into another.
+ Can display Adorners to give the user visual feedback of the operation in progress.
+ Has sensible defaults so that you have to write less code for common operations.

## Want to say thanks?

This framework is free and can be used for free, open source and commercial applications. It's tested and contributed by many people... So mainly hit the :star: button, that's all... thx :squirrel: (:dollar:, :euro:, :beer: or some other gifts are also being accepted...).

## Installation

You can download or fork the source code and compile it. You need at least VS Community 2015 or higher.  
Or: You take the latest version from NuGet: [https://www.nuget.org/packages/gong-wpf-dragdrop](https://www.nuget.org/packages/gong-wpf-dragdrop/)

## Namespace

To use `GongSolutions.WPF.DragDrop` in your application you need to add the namespace to your Xaml files.

```xaml
xmlns:dd="urn:gong-wpf-dragdrop"
```

or

```xaml
xmlns:dd="clr-namespace:GongSolutions.Wpf.DragDrop;assembly=GongSolutions.Wpf.DragDrop"
```

## In action

![screenshot01](./screenshots/2016-09-03_00h51_35.png)

![screenshot02](./screenshots/2016-09-03_00h52_20.png)

![screenshot03](./screenshots/2016-09-03_00h53_03.png)

![screenshot04](./screenshots/2016-09-03_00h53_21.png)

![gif01](./screenshots/DragDropSample01.gif)

## Default Behaviour

A simple example of adding drag/drop to a ListBox:

```xml
<ListBox ItemsSource="{Binding Collection}"
         dd:DragDrop.IsDragSource="True"
         dd:DragDrop.IsDropTarget="True" />
```

Setting the IsDragSource and IsDropTarget attached propeties to True on an ItemsControl such as ListBox enables drag and drop. The default behaviour is to allow re-ordering of items within the control.

If your project contains another ItemsControl with drag/drop enabled in this manner, and it is bound to a collection of the same type, then items can also be dragged and dropped between the controls.

## Adding a Drop Handler

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

For more information, check out the [Showcase application](src/Showcase.WPF.DragDrop.NET45).

#### + [Strong naming](../../wiki/Strong-naming)
#### + [Release History](../../wiki/Release-History)
#### + [License](https://github.com/punker76/gong-wpf-dragdrop/blob/dev/LICENSE)
