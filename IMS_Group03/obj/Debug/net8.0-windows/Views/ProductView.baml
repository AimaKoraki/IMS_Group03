   M S B A M L   `   `   `  ����F  BIMS_Group03, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:IMS_Group03.ViewsIMS_Group03.Views  5      A%clr-namespace:IMS_Group03.ControllersIMS_Group03.Controllers  5      ?$clr-namespace:IMS_Group03.ConvertersIMS_Group03.Converters  5      U QSystem.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a7,clr-namespace:System;assembly=System.RuntimeSystem 5	      #    IMS_Group03.Views.ProductView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:IMS_Group03.Views     5      6ctrl%clr-namespace:IMS_Group03.Controllers     5      ;
converters$clr-namespace:IMS_Group03.Converters     5      ?system,clr-namespace:System;assembly=System.Runtime     5	      -   5           SecondaryBackgroundBrush#��[  5        	TextBrush#��[ 6E     � 	Resources  5      9   3IMS_Group03.Converters.BooleanToVisibilityConverter  	BoolToVis6   0IMS_Group03.Converters.NullToVisibilityConverter  	NullToVis4   .IMS_Group03.Converters.InverseBooleanConverter  
InvertBool�� 5   
   %W   &	       &	      &	 &       5      66     5      63     5      61   5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5       $
��Auto �6   .+�5%   
   � $��0��6   $��0,0,0,15q�6    � ColumnDefinitions 5&      �� 5'      $
��Auto �6#   �� 5(      $��* �6#   .+�5+      �� $��Product Management��6     ViewHeaderStyle#��[ 62   �� 5,      $��1��6    �� Orientation$ 
Horizontal=�6)   $��Right=�6B   .+�5-      �� -   5         ��Name$ RefreshButton��5-   ]   $��Refresh��6     PrimaryActionButtonStyle#��[ 6+   {�6�   �� *IsBusy+ �� 	Converter# [ �� 5.      -   5        $ 	AddButton��5.   ]   $��Add New��6     SuccessActionButtonStyle#��[ 6+   {�6�   �� *IsBusy+# [ �� 5/      -   5        $ 
EditButton��5/   e   $��Edit Selected��6     SecondaryActionButtonStyle#��[ 61   {�6�   �� *IsBusy+# [ �� 50      -   5        $ DeleteButton��50   d   $��Delete Selected��6    	 DangerActionButtonStyle#��[	 63   {�6�   �� *IsBusy+# [ 51      52      &   System.Windows.Controls.DataGrid  55   
   -   5          Name$ ProductsDataGrid��55       $��1��6   $��0,0,0,10q�6:   4  .System.Windows.Automation.AutomationProperties   Name$ List of Products��57       
 DefaultDataGridStyle#��[
 58       $  DefaultDataGridColumnHeaderStyle   ColumnHeaderStyle# [ 59        DefaultDataGridRowStyle	   RowStyle#	 [ 5:        DefaultDataGridCellStyle
   	CellStyle#
 [ 5;      ��56      �� *
Products+ �� SelectedItem 64   �� *SelectedProductGridItem+ �� Mode$ TwoWay=�{�5<      �� *IsBusy+# [    Columns 5=      0  *System.Windows.Controls.DataGridTextColumn  5>      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 70 6H   1	  +System.Windows.Controls.DataGridBoundColumn 	  Binding 61   �� *Id+6H     5?      $	 SKU��6%   $	 140 6J    62   �� *Sku+6J     5@      $
 Name��6%   $ 2* 6L    63   �� *Name+6L     5A      $ Supplier��6%   $
 1.5* 6Y      SortMemberPath$ Supplier.Name��6f    67   �� *Supplier.Name+6f     5B      $	 Qty��6%   $ 80 6V     RightAlignedTextCellStyle 	  ElementStyle# [ 6a    62   �� *QuantityInStock+6a     5C      $ Price��6%   $	 120 6v   # [ 6�    64   �� *Price+ �� StringFormat$ C�� �� TargetNullValue$
 0.00��6�   5E      �� 5I   
   -   5         ��Name$ EditFormBorder��5K      $��2��5I        DisabledBorderBrush#��[ 6   $��1q�6Q    �� CornerRadius$ 5��6e   $��0,5,0,5q�6v    �� Padding$ 20q�6�     FormPanelBackgroundBrush#��[ 5J      y�5L      �� *SelectedProductForForm+# [ ._�5M      � -   5         �Name$ EditFormGrid��5M       � DataContext 6)   �� *SelectedProductForForm+ 5N      �� 5O      $
��Auto �6$   �� 5P      $
��Auto �6$   �� 5Q      $
��Auto �6$    5S      �� 5T      $
��Auto �6'   $	��120��64   �� 5U      $��* �6'   �� 5V      $
��Auto �6'   $	��120��64   �� 5W      $��* �6'   .+�5Z      �� $��0��6   $��0��6)   $��Product Name:��69     LabelStyleDark#��[ 6N   $��Center=�6v   �� 5[      -	   5         ��Name$ NameTextBox��5[   7   $��0��6   $��1��6'   $��	0,0,20,10q�6L     TextBoxDarkStyle#��[ 6_   ��5\      �� *Name+ �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnExceptions$
 True�� �� NotifyOnValidationError$
 True���� 5^      $��1��6   $��0��6)   $
��SKU:��69   #��[ 6E   $��Center=�6m   �� 5_      -
   5        $ 
SkuTextBox��5_   7   $��1��6   $��1��6'   $��	0,0,20,10q�6K   #��[ 6^   ��5`      �� *Sku+$ PropertyChanged=�$
 True��$
 True���� 5b      $��2��6   $��0��6)   $��	Supplier:��69   #��[ 6J   $��Center=�6r   �� 5c      -   5         ��Name$ SupplierComboBox��5c   8   $��2��6   $��1��6(   $��	0,0,20,10q�6R     ComboBoxDarkStyle#��[ 6e    �� SelectedValuePath$ Id��5f      ��5d      �� * DataContext.AvailableSuppliers+  �� RelativeSource  �� ! �� AncestorType"! >�" �� SelectedValue" 5e      �� *
SupplierId+$ PropertyChanged=�# �� MinValue# Z# ��5i      �� 5j      .R�5k      �� ��6(   �� *Name+5l      5o      �� 5q      $��0��6   $��2��6)   $��Quantity in Stock:��69   #��[ 6S   $��Center=�6{   �� 5s      -   5        $ QuantityTextBox��5s   7   $��0��6   $��3��6'   $��0,0,0,10q�6P   $ �� 
IsReadOnly$
$ True��6b     SubtleTextBrush#��[ 6t   % � ToolTip$4% .Stock quantity is managed via Stock Movements.��6�   #��[ 5t   H   ��6   �� *QuantityInStock+$ OneWay=�6H   �� 5v      $��1��6   $��2��6)   $��Price:��69   #��[ 6G   $��Center=�6o   �� 5w      -   5        $ PriceTextBox��5w   7   $��1��6   $��3��6'   $��0,0,0,10q�6M   #��[ 5x   
   ��5y   
   �� *Price+$ PropertyChanged=�$
 True��$
 True��$
 0.00���� 5|      $��2��6   $��2��6)   $��Product ID:��69   #��[ 6L   $��Center=�6t   #��[ 6�   �� 5}      $��2��6   $��3��6)   $��Center=�6M   $��0,0,0,10q�6h   #��[ 6z   ��69   �� *Id+6z   �� 5      $��2��6   $��0��6*   $��4��6:   $ 
Horizontal=�6N   $��Right=�6g   $��Bottom=�6�   $��0,15,0,0q�6�   .+�5�      �� -   5        $ 
SaveButton��5�   ,   $
��Save��6   $��90��6Y   #��[ 6d   �� 5�      -   5        $ CancelButton��5�   _   $��Cancel��6   #��[	 6.   $��90��6�   5�      5�      5�      �� 5�   
   -   5        & ��Name$& ErrorMessageText��5�   !   $��3��6     DangerBrush#��[ 5�      $��0,10,0,0q�6>   $
��Wrap=�6P   $��Center=�6d   $��20��5�   ^   ��5�      �� *ErrorMessage+y�5�      �� *ErrorMessage+# [ 6^   �� 5�   
   -   5        $ LoadingOverlay��5�   \   $��4��6     ModalOverlayBackgroundBrush#��[ 6"   y�5�      �� *IsBusy+# [ ._�5�      �� $��Center=�6   $��Center=�66   .+�5�      � ' � IsIndeterminate$
' True��6   $��60��65   $��60��6@     Accent1Brush#��[ 6L   �� 5�      $��Processing...��6   #��[ 61     FontSizeMedium#��[ 6Y   $��0,10,0,0q�6�   $��SemiBold"�6�   5�      5�      5�      5�      