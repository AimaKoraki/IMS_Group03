// Models/User.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic; // For ICollection

namespace IMS_Group03.Models
{
    public partial class User : ObservableObject
    {
        [ObservableProperty]
        private int _id;

        [ObservableProperty]
        private string _username = string.Empty;

        // PasswordHash and PasswordSalt should NOT be ObservableProperties
        // if they are never directly bound to UI for display.
        // They are typically handled only in the backend/service layer.
        // Keep them as regular properties if they are only for EF Core mapping.
        public string PasswordHash { get; set; } = string.Empty; // Store the hashed password
        public string PasswordSalt { get; set; } = string.Empty; // Store the salt used for hashing

        [ObservableProperty]
        private string _fullName = string.Empty;

        [ObservableProperty]
        private string? _email; // Email can be optional

        [ObservableProperty]
        private string _role = "User"; // Default role

        [ObservableProperty]
        private bool _isActive = true;

        [ObservableProperty]
        private DateTime _dateCreated = DateTime.UtcNow;

        [ObservableProperty]
        private DateTime? _lastLoginDate; // Optional: Track last login

        // Navigation properties (if users create/perform actions)
        public virtual ICollection<PurchaseOrder> CreatedPurchaseOrders { get; set; } = new List<PurchaseOrder>();
        public virtual ICollection<StockMovement> PerformedStockMovements { get; set; } = new List<StockMovement>();

        public User()
        {
            // Ensure non-nullable strings are initialized if not done at declaration
            // Username, FullName, Role are initialized by field initializers or defaults.
            // PasswordHash and PasswordSalt are initialized.
        }
    }
}