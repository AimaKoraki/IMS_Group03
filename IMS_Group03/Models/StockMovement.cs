// Models/StockMovement.cs ---
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace IMS_Group03.Models
{
    public enum MovementType { Addition, Reduction, Adjustment_In, Adjustment_Out, PurchaseReceipt, SaleShipment }

    public partial class StockMovement : ObservableObject
    {
        // ... (existing properties like Id, ProductId, MovementDate, etc. are correct) ...
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private int _productId;
        public virtual Product Product { get; set; } = null!;

        [ObservableProperty]
        private DateTime _movementDate = DateTime.UtcNow;

        [ObservableProperty]
        private MovementType _type;

        [ObservableProperty]
        private int _quantityChanged;

        [ObservableProperty]
        private string _reason = string.Empty;

        [ObservableProperty]
        private int? _sourcePurchaseOrderId;
        public virtual PurchaseOrder? SourcePurchaseOrder { get; set; }


        [ObservableProperty]
        private int? _purchaseOrderItemId; // Foreign key to the specific PO line item
        public virtual PurchaseOrderItem? PurchaseOrderItem { get; set; } // Navigation property


        [ObservableProperty]
        private int? _performedByUserId;
        public virtual User? PerformedByUser { get; set; }
    }
}