   M S B A M L   `   `   `  ����F  BIMS_Group03, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:IMS_Group03.ViewsIMS_Group03.Views  5      A%clr-namespace:IMS_Group03.ControllersIMS_Group03.Controllers  5      ?$clr-namespace:IMS_Group03.ConvertersIMS_Group03.Converters  5      (    "IMS_Group03.Views.UserSettingsView   5      W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      1localclr-namespace:IMS_Group03.Views     5      6ctrl%clr-namespace:IMS_Group03.Controllers     5      ;
converters$clr-namespace:IMS_Group03.Converters     5      -   5           SecondaryBackgroundBrush#��[  5        	TextBrush#��[ 6E     � 	Resources  5      9   3IMS_Group03.Converters.BooleanToVisibilityConverter  	BoolToVis6   0IMS_Group03.Converters.NullToVisibilityConverter  	NullToVis4   .IMS_Group03.Converters.InverseBooleanConverter  
InvertBool�� 5   
   %W   &	       &	      &	 &       5      66     5      63     5      61   5      .��5      � $��20q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6   �� 5      $
��Auto �6   .+�5$   
   � $��0��6   $��0,0,0,15q�6    � ColumnDefinitions 5%      �� 5&      $
��Auto �6#   �� 5'      $��* �6#   .+�5*      �� $��User Management��6     ViewHeaderStyle#��[ 6/   $��Center=�6X   �� 5+      $��1��6    �� Orientation$ 
Horizontal=�6)   $��Right=�6B   .+�5,      �� -   5         ��Name$ RefreshButton��5,   ]   $��Refresh��6     PrimaryActionButtonStyle#��[ 6+   {�6�   �� *IsBusy+ �� 	Converter# [ �� 5-      -   5        $ AddNewButton��5-   b   $��Add New User��6     SuccessActionButtonStyle#��[ 60   {�6�   �� *IsBusy+# [ �� 5.      -   5        $ EditSelectedButton��5.   e   $��Edit Selected��6     SecondaryActionButtonStyle#��[ 61   {�6�   �� *IsBusy+# [ �� 5/      -   5        $ DeleteSelectedButton��5/   d   $��Delete Selected��6    	 DangerActionButtonStyle#��[	 63   {�6�   �� *IsBusy+# [ 50      51      &   System.Windows.Controls.DataGrid  54   
   -   5          Name$ UsersDataGrid��54       $��1��6   $��0,0,0,10q�67    
 DefaultDataGridStyle#��[
 57       $  DefaultDataGridColumnHeaderStyle   ColumnHeaderStyle# [ 58        DefaultDataGridCellStyle   	CellStyle# [ 59        DefaultDataGridRowStyle	   RowStyle#	 [ 5:      ��55      �� *	UsersList+
 �� SelectedItem
 56      �� *SelectedUserForGrid+ �� Mode$ TwoWay=�{�5;      �� *IsBusy+# [    Columns 5<      0  *System.Windows.Controls.DataGridTextColumn  5=      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$ 60 6H   1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Id+6H     5>      $ Username��6%   $ * 6T    67   �� *
Username+6T     5?      $ 	Full Name��6%   $
 1.5* 6U    68   �� *
FullName+6U     5@      $ Email��6%   $
 1.5* 6N    64   �� *Email+6N     5A      $
 Role��6%   $ * 6L    63   �� *Role+6L   4	  .System.Windows.Controls.DataGridCheckBoxColumn	  5B      $ Active��6)   $
 Auto 6c    69   �� *
IsActive+$ OneWay=�   ElementStyle 5C      �� 5D       �� 
TargetType" ��6    .�5E      ��  �� Property � �� �� Value$ Center=�6D   �� 5F       � ��$ Center=�6B   5I         EditingElementStyle 5K      �� 5L      " ��6    .�5M      ��  � ��$ Center=�6D   �� 5N       � ��$ Center=�6B   5P      5R      5T      �� 5X   
   $��2��6     DisabledBorderBrush#��[ 6   $��0,1,0,0q�6Q    �� Padding$ 15q�6k   $��0,10,0,0q�6x     FormPanelBackgroundBrush#��[ 5Y       �� CornerRadius$ 5��5Z      y�5[      �� *SelectedUserForForm+# [ ._�5\      �� .+�5^      �� $��SemiBold"�5_      $��0,0,0,10q�62     SubtleTextBrush#��[ 6D   ��5^      �� *SelectedUserForForm.Id+ �� StringFormat$ User ID: {0}�� �� FallbackValue$ Adding New User��5_   D   � 5`       5a      �� 5b      $
��Auto �6+   $	��110��68   �� 5c      $��* �6+    5e      �� 5f      $
��Auto �6(   �� 5g      $
��Auto �6(   �� 5h      $
��Auto �6(   �� 5i      $
��Auto �6(   �� 5j      $
��Auto �6(   �� 5k      $
��Auto �6(   �� 5l      $
��Auto �6(   .+�5o      �� $��0��6    $��0��6-   $��	Username:��6=     LabelStyleDark#��[ 6N   $��Center=�6v   �� 5p      -   5         ��Name$ UsernameTextBox��5p   ;   $��0��6   $��1��6+     TextBoxDarkStyle#��[ 6�   $��0,0,0,10q�6�   ��6T   �� *UsernameInput+ �� UpdateSourceTrigger$ PropertyChanged=� �� 
IsReadOnly 5q      �� *IsEditingUser+�� 5s      $��1��6    $��0��6-   $��
Full Name:��6=   #��[ 6O   $��Center=�6w   �� 5t      -   5        $ FullNameTextBox��5t   ;   $��1��6   $��1��6+   #��[ 6�   $��0,0,0,10q�6�   ��6T   �� *FullNameInput+$ PropertyChanged=�6�   �� 5v      $��2��6    $��0��6-   $��Email:��6=   #��[ 6K   $��Center=�6s   �� 5w      -	   5        $ EmailTextBox��5w   ;   $��2��6   $��1��6+   #��[ 6�   $��0,0,0,10q�6�   ��6Q   �� *
EmailInput+$ PropertyChanged=�6�   �� 5y      $��3��6    $��0��6-   $��	Password:��6=   #��[ 6N   $��Center=�6v   G� 5z      -
   5         G�Name$ UserPasswordBox��5z   ?   $��3��6"   $��1��6/     PasswordBoxDarkStyle#��[ 6X   $��0,0,0,10q�6�    � ToolTip$M GEnter to set/change. Leave blank to keep current password when editing.��5{   "   �� 5}      $��4��6    $��0��6-   $��Confirm Pwd:��6=   #��[ 6Q   $��Center=�6y   G� 5~      -   5        $ UserConfirmPasswordBox��5~   ?   $��4��6"   $��1��6/   #��[ 6_   $��0,0,0,10q�6�   �� 5�      $��5��6    $��0��6-   $��Role:��6=   #��[ 6J   $��Center=�6r   �� 5�      $��5��6   $��1��6,     ComboBoxDarkStyle#��[ 5�      $��0,0,0,10q�6J   ��5�   <   �� *AvailableRoles+
 5�      �� *SelectedRole+$ PropertyChanged=�5�   J   �� 5�      $��6��6   $��1��6,   $��	Is Active��6<   $��0,5,0,10q�6x   #��[ 6�    d� 	IsChecked 6P   �� *IsUserActiveInput+6�   5�      �� 5�        DangerBrush#��[ 6:   $��0,5,0,10q�6d   $
��Wrap=�6v   ��6   �� *ErrorMessage+y�5�      �� *ErrorMessage+# [ �� 5�      $ 
Horizontal=�6   $��Right=�66   $��0,10,0,0q�6R   .+�5�      �� -   5        $ SaveUserButton��5�   c   $��	Save User��6   #��[ 61   $	��100��6�   �� 5�      -   5        $ ClearFormButton��5�   c   $��
Clear Form��6   #��[	 62   $��5,0,0,0q�6�   $	��100��6�   5�      5�      5�      �� 5�   
   $��3��6   #��[ 6?   $��0,10,0,0q�6i   $
��Wrap=�6{   $��Center=�6�   ��6!   �� *ErrorMessage+y�5�      �� *ErrorMessage+# [ �� 5�   
   -   5         ��Name$ LoadingOverlay��5�   \   $��4��6     ModalOverlayBackgroundBrush#��[ 6"   y�5�      �� *IsBusy+# [ ._�5�      �� $��Center=�6   $��Center=�66   .+�5�      �   � IsIndeterminate$
  True��6   $��60��65   $��60��6@     Accent1Brush#��[ 6L   �� 5�      $��Processing...��6   #��[ 61     FontSizeMedium#��[ 6Y   $��0,10,0,0q�6�   $��SemiBold"�6�   5�      5�      5�      5�      