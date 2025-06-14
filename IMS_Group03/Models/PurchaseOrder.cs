// --- FULLY CORRECTED AND FINALIZED: Models/PurchaseOrder.cs ---
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // FIX: Required for [NotMapped]
using System.Linq; // FIX: Required for .Sum()

namespace IMS_Group03.Models
{
    public enum OrderStatus { Pending = 0, Processing = 1, Shipped = 2, Received = 3, Cancelled = 4 }

    public partial class PurchaseOrder : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private DateTime _orderDate = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime? _expectedDeliveryDate;

        [ObservableProperty]
        private DateTime? _actualDeliveryDate;

        [ObservableProperty]
        private int _supplierId;
        public virtual Supplier Supplier { get; set; } = null!;

        [ObservableProperty]
        private OrderStatus _status;

        [ObservableProperty]
        private string _notes = string.Empty;

        [ObservableProperty]
        private int? _createdByUserId;
        public virtual User? CreatedByUser { get; set; }

        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.UtcNow;

        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        // --- THIS IS THE FIX ---
        // 1. [NotMapped] tells Entity Framework to IGNORE this property. It will not become a database column.
        // 2. The type is changed from 'object' to the correct 'decimal'.
        // 3. The property is now a read-only calculated property that sums the totals of the line items.
        [NotMapped]
        public decimal TotalAmount => PurchaseOrderItems.Sum(item => item.QuantityOrdered * item.UnitPrice);
    }
}