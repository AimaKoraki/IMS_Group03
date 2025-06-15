// Models/PurchaseOrderItem.cs ---
using CommunityToolkit.Mvvm.ComponentModel;

namespace IMS_Group03.Models
{
    public partial class PurchaseOrderItem : ObservableObject
    {
        // FIX: Added a new, single primary key for this table.
        [ObservableProperty]
        private int _purchaseOrderItemId;

        [ObservableProperty]
        private int _purchaseOrderId;
        public virtual PurchaseOrder PurchaseOrder { get; set; } = null!;

        [ObservableProperty]
        private int _productId;
        public virtual Product Product { get; set; } = null!;

        [ObservableProperty]
        private int _quantityOrdered;

        [ObservableProperty]
        private decimal _unitPrice;

        [ObservableProperty]
        private int _quantityReceived;
    }
}