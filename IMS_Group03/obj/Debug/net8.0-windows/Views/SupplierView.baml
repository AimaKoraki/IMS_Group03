   M S B A M L   `   `   `  ����F  BIMS_Group03, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:IMS_Group03.ViewsIMS_Group03.Views  5      A%clr-namespace:IMS_Group03.ControllersIMS_Group03.Controllers  5      ?$clr-namespace:IMS_Group03.ConvertersIMS_Group03.Converters  5      $    IMS_Group03.Views.SupplierView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:IMS_Group03.Views     5      6ctrl%clr-namespace:IMS_Group03.Controllers     5      ;
converters$clr-namespace:IMS_Group03.Converters     5      -   5           SecondaryBackgroundBrush#��[  5        	TextBrush#��[ 6E     � 	Resources  5      9   3IMS_Group03.Converters.BooleanToVisibilityConverter  	BoolToVis6   0IMS_Group03.Converters.NullToVisibilityConverter  	NullToVis4   .IMS_Group03.Converters.InverseBooleanConverter  
InvertBool�� 5   
   %W   &	       &	      &	 &       5      66     5      63     5      61   5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5$   
   � $��0��6   $��0,0,0,15q�6    � ColumnDefinitions 5%      �� 5&      $
��Auto �6#   �� 5'      $��* �6#   .+�5*      �� $��Supplier Management��6     ViewHeaderStyle#��[ 63   $��Center=�6\   �� 5+      $��1��6    �� Orientation$ 
Horizontal=�6)   $��Right=�6B   .+�5,      �� -   5         ��Name$ RefreshButton��5,   ]   $��Refresh��6     PrimaryActionButtonStyle#��[ 6+   {�6�   �� *IsBusy+ �� 	Converter# [ �� 5-      -   5        $ 	AddButton��5-   ]   $��Add New��6     SuccessActionButtonStyle#��[ 6+   {�6�   �� *IsBusy+# [ �� 5/      -   5        $ 
EditButton��5/   e   $��Edit Selected��6     SecondaryActionButtonStyle#��[ 61   {�6�   �� *IsBusy+# [ �� 50      -   5        $ DeleteButton��50   d   $��Delete Selected��6    	 DangerActionButtonStyle#��[	 63   {�6�   �� *IsBusy+# [ 51      52      &   System.Windows.Controls.DataGrid  55   
   -   5          Name$ SuppliersDataGrid��55       $��1��6   $��0,0,0,10q�6;    
 DefaultDataGridStyle#��[
 57       $  DefaultDataGridColumnHeaderStyle   ColumnHeaderStyle# [ 58        DefaultDataGridCellStyle   	CellStyle# [ 59        DefaultDataGridRowStyle	   RowStyle#	 [ 5:      ��56      �� *	Suppliers+
 �� SelectedItem
 65   �� *SelectedSupplierGridItem+ �� Mode$ TwoWay=�{�5;      �� *IsBusy+# [    Columns 5<      0  *System.Windows.Controls.DataGridTextColumn  5=      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 70 6H   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Id+6H     5>      $
 Name��6%   $ 2* 6L    63   �� *Name+6L     5?      $ Contact Person��6%   $
 1.5* 6_    6=   �� *ContactPerson+6_     5@      $ Email��6%   $
 1.5* 6N    64   �� *Email+6N     5A      $ Phone��6%   $
 1.2* 6N    64   �� *Phone+6N   5C      �� 5G   
   -   5         ��Name$ EditFormBorder��5I      $��2��5G        DisabledBorderBrush#��[ 6   $��1q�6Q    �� CornerRadius$ 5��6e   $��0,5,0,5q�6v    �� Padding$ 20q�6�     FormPanelBackgroundBrush#��[ 5H      y�5J      �� *SelectedSupplierForForm+# [ ._�5K      � -   5         �Name$ EditFormGrid��5K       � DataContext 6)   �� *SelectedSupplierForForm+ 5L      �� 5M      $
��Auto �6$   �� 5N      $
��Auto �6$   �� 5O      $
��Auto �6$    5Q      �� 5R      $
��Auto �6'   $	��120��64   �� 5S      $��* �6'   �� 5T      $
��Auto �6'   $	��120��64   �� 5U      $��* �6'   .+�5X      �� $��0��6   $��0��6)   $��Supplier Name:��69     LabelStyleDark#��[ 6O   $��Center=�6w   �� 5Y      -	   5         ��Name$ NameTextBox��5Y   7   $��0��6   $��1��6'   $��	0,0,20,10q�6L     TextBoxDarkStyle#��[ 6_   ��5Z      �� *Name+ �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnExceptions$
 True�� �� NotifyOnValidationError$
 True���� 5\      $��0��6   $��2��6)   $��Contact Person:��69   #��[ 6P   $��Center=�6x   �� 5]      -
   5        $ ContactPersonTextBox��5]   7   $��0��6   $��3��6'   $��0,0,0,10q�6U   #��[ 6g   ��5^      �� *ContactPerson+$ PropertyChanged=��� 5`      $��1��6   $��0��6)   $��Email:��69   #��[ 6G   $��Center=�6o   �� 5a      -   5        $ EmailTextBox��5a   7   $��1��6   $��1��6'   $��	0,0,20,10q�6M   #��[ 6`   ��5b      �� *Email+$ PropertyChanged=�$
 True��$
 True���� 5d      $��1��6   $��2��6)   $��Phone:��69   #��[ 6G   $��Center=�6o   �� 5e      -   5        $ PhoneTextBox��5e   7   $��1��6   $��3��6'   $��0,0,0,10q�6M   #��[ 6_   ��5f      �� *Phone+$ PropertyChanged=��� 5i      $��2��6   $��0��6)   $��Supplier ID:��69   #��[ 6M   $��Center=�6u     SubtleTextBrush#��[ 6�   �� 5j      $��2��6   $��1��6)   $��Center=�6M   $��0,0,0,10q�6h   #��[ 6z   ��69   �� *Id+6z   �� 5l      $��2��6   $��2��6*   $��2��6:   $ 
Horizontal=�6N   $��Right=�6g   $��Bottom=�6�   $��0,15,0,0q�6�   .+�5m      �� -   5        $ 
SaveButton��5m   ^   $
��Save��6   #��[ 6,   $��90��6�   �� 5o      -   5        $ CancelButton��5o   _   $��Cancel��6   #��[	 6.   $��90��6�   5p      5q      5r      �� 5v   
   -   5         ��Name$ ErrorMessageText��5v   !   $��3��6     DangerBrush#��[ 6Y   $��0,10,0,0q�6�   $
��Wrap=�6�   $��Center=�6�   ��6;   �� *ErrorMessage+y�5w      �� *ErrorMessage+# [ �� 5{   
   -   5        $ LoadingOverlay��5{   \   $��4��6     ModalOverlayBackgroundBrush#��[ 6"   y�5|      �� *IsBusy+# [ ._�5}      �� $��Center=�6   $��Center=�66   .+�5~      �  � IsIndeterminate$
 True��6   $��60��65   $��60��6@     Accent1Brush#��[ 6L   �� 5      $��Processing...��6   #��[ 61     FontSizeMedium#��[ 6Y   $��0,10,0,0q�6�   $��SemiBold"�6�   5�      5�      5�      5�      