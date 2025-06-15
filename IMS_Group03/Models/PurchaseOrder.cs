// --- FILE: Models/PurchaseOrder.cs ---

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema; // Required for [NotMapped]
using System.Linq; // Required for .Sum()

namespace IMS_Group03.Models
{
    /// <summary>
    /// Represents the status of a purchase order.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>The order has been created but not yet processed.</summary>
        Pending = 0,
        /// <summary>The order is being prepared by the supplier.</summary>
        Processing = 1,
        /// <summary>The order has been shipped by the supplier.</summary>
        Shipped = 2,
        /// <summary>The order has been received and all items are accounted for.</summary>
        Received = 3,
        /// <summary>The order has been cancelled.</summary>
        Cancelled = 4
    }

    /// <summary>
    /// Represents a purchase order sent to a supplier to procure products.
    /// </summary>
    public partial class PurchaseOrder : ObservableObject
    {
        /// <summary>
        /// Gets or sets the unique identifier for the purchase order. Primary Key.
        /// </summary>
        [ObservableProperty]
        private int _id;

        /// <summary>
        /// Gets or sets the date and time when the order was placed.
        /// </summary>
        [ObservableProperty]
        private DateTime _orderDate = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the date on which the delivery is expected to arrive. Can be null.
        /// </summary>
        [ObservableProperty]
        private DateTime? _expectedDeliveryDate;

        /// <summary>
        /// Gets or sets the actual date on which the delivery arrived. Can be null until the order is received.
        /// </summary>
        [ObservableProperty]
        private DateTime? _actualDeliveryDate;

        /// <summary>
        /// Gets or sets the foreign key for the Supplier from whom this order was placed.
        /// </summary>
        [ObservableProperty]
        private int _supplierId;

        /// <summary>
        /// Gets or sets the navigation property for the associated Supplier.
        /// Initialized to null! to satisfy the non-nullable compiler check, as it is expected
        /// to be populated by Entity Framework Core during data retrieval.
        /// </summary>
        public virtual Supplier Supplier { get; set; } = null!;

        /// <summary>
        /// Gets or sets the current status of the order (e.g., Pending, Shipped, Received).
        /// </summary>
        [ObservableProperty]
        private OrderStatus _status;

        /// <summary>
        /// Gets or sets any additional notes or comments related to the purchase order.
        /// </summary>
        [ObservableProperty]
        private string _notes = string.Empty;

        /// <summary>
        ///Gets or sets the foreign key for the User who created the purchase order.Can be null.
        /// </summary>
        [ObservableProperty]
        private int? _createdByUserId;

        /// <summary>
        /// Gets or sets the navigation property for the User who created the order.
        /// </summary>
        public virtual User? CreatedByUser { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when this record was created.
        /// </summary>
        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the UTC date and time when this record was last updated.
        /// </summary>
        [ObservableProperty]
        private DateTime _lastUpdated = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the collection of individual line items included in this purchase order.
        /// </summary>
        public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();

        /// <summary>
        /// A calculated property that sums the total cost of all items in the purchase order.
        /// This property is calculated on-the-fly and is not stored in the database.
        /// </summary>
        /// <remarks>
        /// The value is derived by summing the (QuantityOrdered * UnitPrice) of all <see cref="PurchaseOrderItems"/>.
        /// The <c>[NotMapped]</c> attribute is used to explicitly tell Entity Framework Core to ignore this property
        /// during database schema creation and migrations.
        /// </remarks>
        [NotMapped]
        public decimal TotalAmount => PurchaseOrderItems?.Sum(item => item.QuantityOrdered * item.UnitPrice) ?? 0;
    }
}