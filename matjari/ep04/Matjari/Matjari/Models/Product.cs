namespace Matjari.Models;

public class Product
{
    public int Id { get; set; }

    // كود المحل، وهو غير الباركود.
    // الباركود رقم الشركة، والكود رقمك أنت.
    public string Code { get; set; } = "";
    public string Barcode { get; set; } = "";
    public string Name { get; set; } = "";
    public int? CategoryId { get; set; }

    // ليس عمودًا في الجدول: يأتي بالربط من التصنيفات.
    public string CategoryName { get; set; } = "";

    public string Unit { get; set; } = "قطعة";
    public decimal CostPrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal ReorderLevel { get; set; }
    public bool IsActive { get; set; } = true;

    public override string ToString() => Name;

}