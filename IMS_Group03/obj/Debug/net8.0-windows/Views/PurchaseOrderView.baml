   M S B A M L   `   `   `  ����F  BIMS_Group03, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:IMS_Group03.ViewsIMS_Group03.Views  5      A%clr-namespace:IMS_Group03.ControllersIMS_Group03.Controllers  5      7 clr-namespace:IMS_Group03.ModelsIMS_Group03.Models  5      ?$clr-namespace:IMS_Group03.ConvertersIMS_Group03.Converters  5	      U QSystem.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a7,clr-namespace:System;assembly=System.RuntimeSystem 5
      )    #IMS_Group03.Views.PurchaseOrderView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:IMS_Group03.Views     5      6ctrl%clr-namespace:IMS_Group03.Controllers     5      3models clr-namespace:IMS_Group03.Models     5      ;
converters$clr-namespace:IMS_Group03.Converters     5	      ?system,clr-namespace:System;assembly=System.Runtime     5
      -   5           SecondaryBackgroundBrush#��[  5        	TextBrush#��[ 6E     � 	Resources  5      9   3IMS_Group03.Converters.BooleanToVisibilityConverter  	BoolToVis6   0IMS_Group03.Converters.NullToVisibilityConverter  	NullToVis4   .IMS_Group03.Converters.InverseBooleanConverter  
InvertBoolD   >IMS_Group03.Converters.OrderStatusToReceiveVisibilityConverter + 'OrderStatusToReceiveVisibilityConverterC   =IMS_Group03.Converters.OrderStatusToCancelVisibilityConverter * &OrderStatusToCancelVisibilityConverter�� 5   
   %�   &	       &	      &	 &     &	 9     &	 L       5      66     5      63     5      61     5      6A     5      6@   5      .��5      � $��15q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5       $
��Auto �6   �� 5!      $
��Auto �6   .+�5$   
   � $��0��6   $��0,0,0,15q�6    � ColumnDefinitions 5%      �� 5&      $
��Auto �6#   �� 5'      $��* �6#   .+�5*      �� $��Purchase Orders��6     ViewHeaderStyle#��[ 6/   �� 5+      $��1��6    �� Orientation$ 
Horizontal=�6)   $��Right=�6B   .+�5-      �� -   5         ��Name$ RefreshButton��5-   [   $��Refresh��6     PrimaryActionButtonStyle#��[ 6)   {�6�   �� *IsBusy+ �� 	Converter# [ �� 5.      -   5        $ NewOrderButton��5.   h   $��New Purchase Order��6    	 SuccessActionButtonStyle#��[	 66   {�6�   �� *IsBusy+# [ 5/      50      &   System.Windows.Controls.DataGrid  53   
   -   5          Name$ PurchaseOrdersDataGrid��53       $��1��6   $��0,0,0,10q�6@    
 DefaultDataGridStyle#��[
 55       $  DefaultDataGridColumnHeaderStyle   ColumnHeaderStyle# [ 56        DefaultDataGridRowStyle   RowStyle# [ 57        DefaultDataGridCellStyle	   	CellStyle#	 [ 58      ��54      �� *PurchaseOrders+
 �� SelectedItem
 6:   �� *SelectedOrderForForm+ �� Mode$ TwoWay=�{�59      �� *IsBusy+# [    Columns 5:      0  *System.Windows.Controls.DataGridTextColumn  5;      ,  &System.Windows.Controls.DataGridColumn   Header$ PO ID��6%      Width5	  /System.Windows.Controls.DataGridLengthConverter$ 80	 6K   1
  +System.Windows.Controls.DataGridBoundColumn 
  Binding 64   �� *Id+6K     5<      $ 
Order Date��6%   $	 120	 6q    69   �� *	OrderDate+ �� StringFormat$ dd-MMM-yyyy��6q     5=      $ Supplier��6%   $
 1.5*	 6Y      SortMemberPath$ Supplier.Name��6f    67   �� *Supplier.Name+6f     5>      $ Status��6%   $	 120	 6P    65   �� *Status+6P     5?      $ Expected Delivery��6%   $	 140	 6�    6@   �� *ExpectedDeliveryDate+$ dd-MMM-yyyy�� �� TargetNullValue$	 N/A��6�     5A      $ Total Amount��6%   $	 130	 6k     RightAlignedTextCellStyle 
  ElementStyle# [ 6w    6;   �� *TotalAmount+$ C��6w   4  .System.Windows.Controls.DataGridTemplateColumn  5B      $ Actions��6)   $
 Auto	 6:      CellTemplate 5C      �� 5D      .R�5E      �� $ 
Horizontal=�6)   .+�5F   "   �� -   5        $
��Edit��5F   )     SmallLinkButtonStyle#��[ 68   ��6�   �� �� 5H   "   -   5        $��Receive��5H   )   #��[	 6;   $	��5,0q�6m   ��5I   J   �� y�5J   )   �� *Status+# [ �� 5L   "   -   5        $��	Cancel PO��5L   )     DangerActionButtonStyle#��[ 6=   ��5M   F   �� y�5N   )   �� *Status+# [ 5O      5P      5R      5T      �� 5W   
   -   5         ��Name$ EditPoFormBorder��5Y      $��2��5W        DisabledBorderBrush#��[ 6   $��1q�6Q    �� CornerRadius$ 5��6e   $��0,5,0,5q�6v    �� Padding$ 20q�6�     FormPanelBackgroundBrush#��[ 5X      y�5Z      �� *SelectedOrderForForm+# [ ._�5[      � -	   5         �Name$ EditPoFormGrid��5[       � DataContext 6+   �� *SelectedOrderForForm+ 5\      �� 5]      $
��Auto �6$   �� 5^      $
��Auto �6$   �� 5_      $��* �6$   �� 5`      $
��Auto �6$   .+�5c      � $��0��6   $��0,0,0,15q�6$    5d      �� 5e      $
��Auto �6+   $	��120��68   �� 5f      $��* �6+   �� 5g      $
��Auto �6+   $	��120��68   �� 5h      $��* �6+    5j      �� 5k      $
��Auto �6(   �� 5l      $
��Auto �6(   .+�5o      �� $��0��6    $��0��6-   $��PO ID:��6=     LabelStyleDark#��[ 6K   $��Center=�6s   �� 5q      $��0��6    $��1��6-   $��Center=�6j   $��	0,0,20,10q�6�     SubtleTextBrush#��[ 6�   ��6=   �� *Id+$ (New)��6�   �� 5r      $��0��6    $��2��6-   $��Order Date:��6=   #��[ 6P   $��Center=�6x   (  "System.Windows.Controls.DatePicker  5t      $��0��6!   $��3��6.   $��0,0,0,10q�6>     DatePickerDarkStyle#��[ 6P      SelectedDate 6}   �� *	OrderDate+ �� UpdateSourceTrigger$ PropertyChanged=��� 5u      $��1��6    $��0��6-   $��	Supplier:��6=   #��[ 6N   $��Center=�6v   �� 5w      $��1��6   $��1��6,   $��0,0,20,0q�6<     ComboBoxDarkStyle#��[ 6N    �� SelectedValuePath$ Id��5z      ��5x      �� * DataContext.AvailableSuppliers+ �� RelativeSource ��  �� AncestorType" >� �� SelectedValue 5y      �� *
SupplierId+$ PropertyChanged=���5{      �� 5|      .R�5}   "   �� ��6,   �� *Name+5~      5�      �� 5�      $��1��6    $��2��6-   $��Expected Delivery:��6=   #��[ 6W   $��Center=�6     5�      $��1��6!   $��3��6.   $��0,0,0,0q�6>   #��[ 6O    6|   �� *ExpectedDeliveryDate+$ PropertyChanged=�5�      �� 5�      -
   5        $ AddItemButton��5�      $��1��5�      $��
+ Add Item��6&   $
��Left=�6;     SecondaryActionButtonStyle#��[ 6V   $	��8,4q�6�   $��0,0,0,8q�6�   5�   0     5�      -   5        $ OrderItemsDataGrid��5�   (   $��2��6   $��0,0,0,10q�6D   $	��150��6V   $	��300��6f   #��[
 5�      # [ 5�      # [ 5�      #	 [ 5�      ��5�      �� * DataContext.EditableOrderItems+ �� " >� 5�        5�      $ Product��61   $ 2*	 6B    5�      �� 5�   "   .R�5�   &   �� #��[ 6/   $ Id��5�   /   ��5�   /   �� *AvailableProductsForSelection+ 5�   /   �� *	ProductId+$ PropertyChanged=���5�   *   �� 5�   .   .R�5�   2   �� ��6<   �� *Name+5�   /   5�   '   5�   #   5�        5�      $	 SKU��6-   $ *	 6f       
IsReadOnly$
  True��6p    6:   �� *
ProductSku+$ OneWay=�6p     5�      $	 Qty��61   $	 100	 6>    5�      �� 5�   "   .R�5�   &   ��   TextBoxDarkStyle#��[ 6�   $��60��5�   .   ! �� TextAlignment$! Right=�6<   ��5�   .   �� *
Quantity+$ PropertyChanged=�$ 0��5�   <   5�   #   5�        5�      $ 
Unit Price��61   $	 120	 6E    5�      �� 5�   "   .R�5�   &   �� #��[ 6�   $��80��5�   .   $! Right=�6<   ��5�   .   �� *	UnitPrice+$ C��$ PropertyChanged=�$
 0.00��5�   <   5�   #   5�        5�      $ Total��6-   $	 130	 6x   $
  True��6�   # [ 6�    6<   �� *
TotalPrice+$ C��$ OneWay=�6�     5�      $
 Auto	 61    5�      �� 5�   "   .R�5�   &   �� -   5        $��Remove��5�   -   #��[ 6>   ��5�   L   �� 5�   #   5�      5�      �� 5�      $��3��6   $ 
Horizontal=�6*   $��Right=�6C   $��0,15,0,0q�6_   .+�5�      �� -   5        $ SavePoButton��5�   m   $��Save Purchase Order��6   #��[	 6;   $	��160��6�   �� 5�      -   5        $ CancelPoEditButton��5�   g   $��Cancel Edit��6   #��[ 63   $	��100��6�   5�      5�      5�      �� 5�   
   -   5        " ��Name$" ErrorMessageText��5�   !   $��3��6     DangerBrush#��[ 6Y   $��0,10,0,0q�6�   $
��Wrap=�6�   $��Center=�6�   ��6;   �� *ErrorMessage+y�5�      �� *ErrorMessage+# [ �� 5�   
   -   5        $ LoadingOverlay��5�   \   $��4��6     ModalOverlayBackgroundBrush#��[ 6"   y�5�      �� *IsBusy+# [ ._�5�      �� $��Center=�6   $��Center=�66   .+�5�      � # � IsIndeterminate$
# True��6   $��60��65   $��60��6@     Accent1Brush#��[ 6L   �� 5�      $��Processing...��6   #��[ 61     FontSizeMedium#��[ 6Y   $��0,10,0,0q�6�   $��SemiBold"�6�   5�      5�      5�      5�      