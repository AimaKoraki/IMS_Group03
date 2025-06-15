// --- FILE: Models/Product.cs ---

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;

namespace IMS_Group03.Models
{
    /// <summary>
    /// Represents a product in the inventory.
    /// This class is designed to be used as both an Entity Framework Core entity
    /// and an observable object for MVVM data binding.
    /// </summary>
    public partial class Product : ObservableObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the product. This is the primary key.
        /// </summary>
        [ObservableProperty]
        private int _id;

        /// <summary>
        /// Gets or sets the display name of the product.
        /// </summary>
        [ObservableProperty]
        private string _name = string.Empty;

        /// <summary>
        /// Gets or sets the Stock Keeping Unit (SKU), a unique code used for inventory tracking.
        /// </summary>
        [ObservableProperty]
        private string _sku = string.Empty;

        /// <summary>
        /// Gets or sets a detailed description of the product.
        /// </summary>
        [ObservableProperty]
        private string _description = string.Empty;

        /// <summary>
        //Gets or sets the current number of units available in inventory.
        /// </summary>
        [ObservableProperty]
        private int _quantityInStock;

        /// <summary>
        /// Gets or sets the selling price per unit of the product.
        /// It's recommended to use decimal for currency values to avoid floating-point inaccuracies.
        /// </summary>
        [ObservableProperty]
        private decimal _price;

        /// <summary>
        /// Gets or sets the inventory level at which this product is considered 'low stock',
        /// used for generating alerts or reports.
        /// </summary>
        [ObservableProperty]
        private int _lowStockThreshold = 10;

        /// <summary>
        /// Gets or sets the foreign key for the associated Supplier. Can be null if no supplier is linked.
        /// </summary>
        [ObservableProperty]
        private int? _supplierId;

        /// <summary>
        /// Gets or sets the navigation property for the product's supplier.
        /// This is used by Entity Framework Core to load the related Supplier object.
        /// </summary>
        public virtual Supplier? Supplier { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the product record was first created.
        /// </summary>
        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the UTC date and time when the product record was last modified.
        /// </summary>
        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets a collection of purchase order line items associated with this product.
        /// Represents the "many" side of a one-to-many relationship.
        /// </summary>
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        /// <summary>
        /// Gets or sets a collection of stock movements (e.g., additions, removals) for this product.
        /// Represents the "many" side of a one-to-many relationship.
        /// </summary>
        public virtual ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Product"/> class with default values.
        /// Field initializers are used for properties like Name, SKU, and collections.
        /// </summary>
        public Product()
        {
            // The constructor is intentionally left empty.
            // Properties are initialized using field initializers for cleaner, more modern C# syntax.
        }
    }
}