using System;
using System.ComponentModel.DataAnnotations;

namespace Manufacturing.Models
{
    public class MaterialReceipt
    {
        [Key]
        public int ReceiptId { get; set; }

        public string? ReceiptNumber { get; set; }

        [Required(ErrorMessage = "Please select a material")]
        public int MaterialId { get; set; }

        public string? MaterialName { get; set; }

        public string? MaterialCode { get; set; }

        public int SupplierId { get; set; }

        public string? SupplierName { get; set; }

        [Required(ErrorMessage = "Please enter received quantity")]
        public int ReceivedQuantity { get; set; }

        public string? Unit { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalCost { get; set; }

        public DateTime ReceiptDate { get; set; }

        public string? BatchNumber { get; set; }

        public string? PurchaseOrderNo { get; set; }

        public string? QualityInspectionStatus { get; set; }

        public string? StorageLocation { get; set; }

        public string? ReceivedBy { get; set; }

        public string? Status { get; set; }

        public string? Remarks { get; set; }

        public DateTime CreatedDate { get; set; }

        public string? ReferenceNo { get; set; }

        public DateTime ReceiveDate { get; set; }
    }
}
