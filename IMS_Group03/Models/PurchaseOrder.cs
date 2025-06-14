// --- CORRECTED AND FINALIZED: Models/PurchaseOrder.cs ---
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

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

        // Foreign Key for Supplier
        [ObservableProperty]
        private int _supplierId;
        // --- FIX 1: Initialized non-nullable navigation property with 'null!' for EF Core ---
        public virtual Supplier Supplier { get; set; } = null!;

        [ObservableProperty]
        private OrderStatus _status;

        [ObservableProperty]
        private string _notes = string.Empty;

        // Foreign Key for User
        [ObservableProperty]
        private int? _createdByUserId;
        // --- FIX 2: Made navigation property nullable to match the foreign key (int?) ---
        public virtual User? CreatedByUser { get; set; }


        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.UtcNow;

        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
    }
}