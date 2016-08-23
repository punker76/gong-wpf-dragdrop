using Faker;

namespace Showcase.WPF.DragDrop.Models
{
  public class TabItemModel
  {
    public TabItemModel(int itemIndex)
    {
      this.Header = $"TabItem {itemIndex}";
      this.Content = Faker.Lorem.Paragraph();
    }

    public string Header { get; set; }
    public string Content { get; set; }
  }
}