   M S B A M L   `   `   `  ����F  BIMS_Group03, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:IMS_Group03.ViewsIMS_Group03.Views  5      A%clr-namespace:IMS_Group03.ControllersIMS_Group03.Controllers  5      ?$clr-namespace:IMS_Group03.ConvertersIMS_Group03.Converters  5      )    #IMS_Group03.Views.StockMovementView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:IMS_Group03.Views     5      6ctrl%clr-namespace:IMS_Group03.Controllers     5      ;
converters$clr-namespace:IMS_Group03.Converters     5      -   5           SecondaryBackgroundBrush#��[  5        	TextBrush#��[ 6E     � 	Resources  5      9   3IMS_Group03.Converters.BooleanToVisibilityConverter  	BoolToVis6   0IMS_Group03.Converters.NullToVisibilityConverter  	NullToVis4   .IMS_Group03.Converters.InverseBooleanConverter  
InvertBool�� 5   
   %W   &	       &	      &	 &       5      66     5      63     5      61   5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5!      $
��Auto �6   �� 5#      $
��Auto �6   �� 5%      $
��Auto �6   .+�5*   
   �� $��0��6   $��Stock Movement Ledger��6!     ViewHeaderStyle#��[ 6>   $��0,0,0,15q�6g   �� 5-   
   $��1��6    �� Orientation$ 
Horizontal=�6"   $��0,0,0,10q�6;   .+�5/      �� $!��View movements for product:��6   $��Center=�6;   $��0,0,10,0q�6V     LabelStyleDark#��[ 6h   �� 51      -   5         ��Name$ ProductFilterComboBox��51      $	��300��66     ComboBoxDarkStyle#��[ 6B    �� SelectedValuePath$ Id��54      ��52      �� *AvailableProducts+ �� SelectedItem 53      �� *SelectedProductFilter+ �� Mode$ TwoWay=���55      �� 56      .R�57      �� ��6$   �� *Name+58      5:      5;      &   System.Windows.Controls.DataGrid  5?   
   -   5          Name$ StockMovementsDataGrid��5?       $��2��6   $��0,0,0,10q�6@     DefaultDataGridStyle#��[ 5A       $	  DefaultDataGridColumnHeaderStyle   ColumnHeaderStyle# [	 5B       
 DefaultDataGridRowStyle	   RowStyle#	 [
 5C        DefaultDataGridCellStyle
   	CellStyle#
 [ 5D      ��5@      �� *StockMovements+{�5E      �� *IsBusy+ �� 	Converter# [    Columns 5F      0  *System.Windows.Controls.DataGridTextColumn  5G      ,  &System.Windows.Controls.DataGridColumn   Header$
 Date��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 160 6u      SortDirection 
Descending6�   1  +System.Windows.Controls.DataGridBoundColumn   Binding 63   �� *MovementDate+ �� StringFormat$ yyyy-MM-dd HH:mm��6�     5H      $ Product SKU��6%   $	 120 6Z    6:   �� *Product.Sku+6Z     5I      $ Product Name��6%   $ 2* 6\    6;   �� *Product.Name+6\     5J      $
 Type��6%   $	 130 6L    63   �� *Type+6L     5K      $ Qty Changed��6%   $	 100 6^     RightAlignedTextCellStyle   ElementStyle# [ 6j    6:   �� *QuantityChanged+6j     5M      $ Reason / PO Ref��6%   $
 2.5* 6Y    6>   �� *Reason+6Y   5O      �� 5R   
   $��3��6   $��Manual Stock Adjustment��6!     SubheaderStyle#��[ 6@   �� 5V   
   -   5         ��Name$ AdjustmentFormBorder��5X      $��4��5V        DisabledBorderBrush#��[ 6   $��1q�6Q    �� CornerRadius$ 5��6e    �� Padding$ 15q�6v     FormPanelBackgroundBrush#��[ 5W      ._�5Y      � -   5         �Name$ AdjustmentFormGrid��5Y       5Z      �� 5[      $
��Auto �6$   �� 5\      $
��Auto �6$   �� 5]      $
��Auto �6$    � ColumnDefinitions 5_      �� 5`      $
��Auto �6'   $	��120��64   �� 5a      $��* �6'   �� 5b      $
��Auto �6'   $	��120��64   �� 5c      $��* �6'   .+�5f      �� $��0��6   $��0��6)   $��Product to Adjust:��69   #��[ 6S   $��Center=�6{   �� 5g      -   5        $ AdjustmentProductComboBox��5g   8   $��0��6   $��1��6(   $��	0,0,20,10q�6[   #��[ 6n   $ Id��5j      ��5h      �� *AvailableProducts+ �� SelectedValue 5i      �� *AdjustmentProductId+ �� UpdateSourceTrigger$ PropertyChanged=���5k      �� 5l      .R�5m      �� ��6(   �� *Name+5n      5p      �� 5r      $��0��6   $��2��6)   $��New Actual Quantity:��69   #��[ 6U   $��Center=�6}   �� 5s      -   5         ��Name$ AdjustmentQuantityTextBox��5s   7   $��0��6   $��3��6'   $��0,0,0,10q�6Z     TextBoxDarkStyle#��[ 6l   ��5t      �� *AdjustmentNewQuantityInput+$ PropertyChanged=��� 5x      $��1��6   $��2��6)   $��Reason:��69   #��[ 6H   $��Center=�6p   �� 5y      -   5        $ AdjustmentReasonTextBox��5y   K   $��1��6   $��3��6'   $��1��67   $��0,0,0,10q�6l   #��[ 6~    �� TextWrapping$
 Wrap=�5z   a    �� AcceptsReturn$
 True��6u   $��40��6�   ��6   �� *AdjustmentReason+$ PropertyChanged=�6�   �� 5|      $��2��6   $��0��6*   $��4��6:   $ 
Horizontal=�6N   $��Right=�6g   $��0,10,0,0q�6�   .+�5~      �� -	   5         ��Name$ PerformAdjustmentButton��5�      $��Perform Adjustment��5~        SuccessActionButtonStyle#��[ 5      5�      �� 5�      -
   5        $ ClearAdjustmentFormButton��5�      $��
Clear Form��5�        DangerActionButtonStyle#��[ 5�      5�      5�      5�      5�      �� 5�   
   $��5��6     DangerBrush#��[ 6?   $��0,10,0,0q�6i   $
��Wrap=�6{   $��Center=�6�   ��6!   �� *ErrorMessage+y�5�      �� *ErrorMessage+# [ �� 5�   
   -   5        $ LoadingOverlay��5�   \   $��6��6     ModalOverlayBackgroundBrush#��[ 6"   y�5�      �� *IsBusy+# [ ._�5�      �� $��Center=�6   $��Center=�66   .+�5�      �  � IsIndeterminate$
 True��6   $��60��65   $��60��6@     Accent1Brush#��[ 6L   �� 5�      $��Processing...��6   #��[ 61     FontSizeMedium#��[ 6Y   $��0,10,0,0q�6�   $��SemiBold"�6�   5�      5�      5�      5�      