using Faker;

namespace Showcase.WPF.DragDrop.Models
{
  public class DataGridRowModel
  {
    public string Name { get; set; } = Faker.Name.FullName(NameFormats.Standard);
    public string StreetName { get; set; } = Faker.Address.StreetName();
    public string City { get; set; } = Faker.Address.City();
  }
}