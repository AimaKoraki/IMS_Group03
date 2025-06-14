// --- FULLY CORRECTED AND FINALIZED: Models/PurchaseOrder.cs ---
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Required for [NotMapped]
using System.Linq; // Required for .Sum()

namespace IMS_Group03.Models
{
    public enum OrderStatus { Pending = 0, Processing = 1, Shipped = 2, Received = 3, Cancelled = 4 }

    // Using partial class is correct for source generators like CommunityToolkit.Mvvm
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
        public virtual Supplier Supplier { get; set; } = null!; // Correctly initialized for non-nullable reference types

        [ObservableProperty]
        private OrderStatus _status;

        [ObservableProperty]
        private string _notes = string.Empty;

        // Foreign Key for User
        [ObservableProperty]
        private int? _createdByUserId;
        public virtual User? CreatedByUser { get; set; } // Correctly nullable

        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.UtcNow;

        // This is a collection and doesn't need to be an ObservableProperty
        // unless you are replacing the entire collection at runtime and need UI to update.
        // For adding/removing items, you'd use an ObservableCollection in the ViewModel.
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        // --- FIX: The missing TotalAmount property is added here. ---
        /// <summary>
        /// A calculated property that sums the total price of all line items.
        /// This is not mapped to the database; it's calculated in memory for UI display.
        /// </summary>
        [NotMapped] // IMPORTANT: This tells EF Core NOT to create a 'TotalAmount' column in the database.
        public decimal TotalAmount => PurchaseOrderItems?.Sum(item => item.QuantityOrdered * item.UnitPrice) ?? 0;
        // --- END OF FIX ---
    }
}