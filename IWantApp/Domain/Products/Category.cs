namespace IWantApp.Domain.Products;

public class Category: Entity
{
    public string Name { get; private set; }
    public bool Active { get; private set; }

    public Category(string name, string createdBy, string editedBy)
    {
        Name = name;
        Active = true;
        EditedBy = editedBy;
        CreatedBy = createdBy;
        EditedOn = DateTime.Now;
        CreatedOn = DateTime.Now;

        Validate();
    }

    private void Validate()
    {
        var contract = new Contract<Category>()
                    .IsNotNullOrEmpty(Name, "Name")
                    .IsNotNullOrEmpty(CreatedBy, "CreatedBy")
                    .IsNotNullOrEmpty(EditedBy, "EditedBy")
                    .IsGreaterOrEqualsThan(Name, 3, "Name");
        AddNotifications(contract);
    }

    public void EditInfo(string name, bool active, string editedBy)
    {
        Active = active;
        Name= name;
        EditedBy = editedBy;
        EditedOn= DateTime.Now;
        Validate();
    }
}
