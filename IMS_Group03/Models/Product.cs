// Models/Product.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace IMS_Group03.Models
{
    public partial class Product : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _name = string.Empty; // Initialize to empty string

        [ObservableProperty]
        private string _sku = string.Empty;  // Initialize to empty string

        [ObservableProperty]
        private string _description = string.Empty; // Initialize to empty string

        [ObservableProperty]
        private int _quantityInStock; // int is non-nullable value type, defaults to 0

        [ObservableProperty]
        private decimal _price; // decimal is non-nullable value type, defaults to 0.0m

        [ObservableProperty]
        private int _lowStockThreshold = 10; // Example default, or 0

        [ObservableProperty]
        private int? _supplierId; // Nullable int? is fine

        // For Supplier, you have a few choices:
        // Choice A: Initialize to a new, empty Supplier (if that makes sense in your domain)
        // public virtual Supplier Supplier { get; set; } = new Supplier(); 
        // This requires Supplier to also have a parameterless constructor and initialize its own non-nullable fields.

        // Choice B: Mark Supplier as nullable if a Product can truly exist without a Supplier reference initially.
        // This often aligns better with a nullable _supplierId.
        public virtual Supplier? Supplier { get; set; } // Changed to Supplier?

        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow; // Initialize with a sensible default

        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.UtcNow; // Initialize with a sensible default

        // Collections are correctly initialized
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

        // Optional: Add a parameterless constructor to explicitly show initialization
        // (though field initializers above are often sufficient)
        public Product()
        {
            // _name, _sku, _description, _dateCreated, _lastUpdated are already handled by field initializers.
            // _quantityInStock and _price default to 0.
            // _lowStockThreshold has a default.
            // _supplierId is nullable.
            // Supplier is now nullable or would need initialization if it remained non-nullable.
            // PurchaseOrderItems and StockMovements are initialized.
        }
    }
}