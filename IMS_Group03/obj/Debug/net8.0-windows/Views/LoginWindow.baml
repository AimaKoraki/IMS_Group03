   M S B A M L   `   `   `  ����F  BIMS_Group03, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null5clr-namespace:IMS_Group03.ViewsIMS_Group03.Views  5   	   A%clr-namespace:IMS_Group03.ControllersIMS_Group03.Controllers  5	   	   ?$clr-namespace:IMS_Group03.ConvertersIMS_Group03.Converters  5
   	   #    IMS_Group03.Views.LoginWindow   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   1localclr-namespace:IMS_Group03.Views     5   	   6ctrl%clr-namespace:IMS_Group03.Controllers     5	   	   ;
converters$clr-namespace:IMS_Group03.Converters     5
   	     � Icon$  /Assets/Images/icon.ico��5   	    � Title$) #Inventory Management System - Login��5   	   $	��550��5   	   $	��450��6    � WindowStartupLocation$ CenterScreen=�5   	    � WindowStyle$
 None=�5   	    � AllowsTransparency$
 True��6   
������  � 	Resources 5      9   3IMS_Group03.Converters.BooleanToVisibilityConverter   	BoolToVis6   0IMS_Group03.Converters.NullToVisibilityConverter  	NullToVis�� 5   
   %:   &	        &	        5      66     5      63   5      .��5      ��   PrimaryBackgroundBrush#��[ 6    �� CornerRadius$ 10��6B    �� Padding$ 1q�6T   ._�5   
   ��   PanelBackgroundBrush#��[ 6   $ 9��6D   ._�5      �  � RowDefinitions 5      �� 5      $
��Auto �6$   �� 5       $��* �6$   .+�5$      �� -   5        $��0��5$      
������ $ 9,9,0,0��6^   $��40��6u   ._�5%      �� -   5        $	����5%      $��Segoe MDL2 Assets)�6)   $��Right=�6H   $	��Top=�6d   $��40��5&      $��40��6(   
������ 
�������$��0q�6`   	 � ToolTip$	 Close Application��5'   7   5(      �� 5*      $��1��6   $��
40,0,40,40q�6*   $��Center=�6>   .+�5+      �� $��Welcome Back��6      LoginTitleStyle#��[ 64   �� 5,      $ ��Please sign in to continue��6      LoginSubtitleStyle#��[ 6B   �� 5.      $��Username��6      LabelStyleDark#��[ 60   �� 5/        TextBoxDarkStyle#��[ 6]   $��0,0,0,15q�6�   ��6   �� *
Username+
 �� UpdateSourceTrigger$
 PropertyChanged=�6�   �� 51      $��Password��6    #��[ 60   G� 52      -   5         G�Name$ UserPasswordBox��52   "     PasswordBoxDarkStyle#��[ 6;   $��0,0,0,25q�6i   �� 55       	 DangerBrush#��[	 6>   $��0,0,0,10q�6h   $
��Wrap=�56       ��55       �� *ErrorMessage+y�56   4   �� *ErrorMessage+ �� 	Converter# [ �� 58      -   5        $��Login��58       
 PrimaryActionButtonStyle#��[
 6-    �� 	IsDefault$
 True��6y   59      �� 5<      $��1��6     ModalOverlayBackgroundBrush#��[ 6&   $ 9��6`   y�5=      �� *IsBusy+# [  ._�5>      �� $��Center=�6!   $��Center=�6>   .+�5?      �  � IsIndeterminate$
 True��6&   $��50��6=   $��50��6H     Accent1Brush#��[ 6T   �� 5@      $��Authenticating...��6$     	TextBrush#��[ 6=   $��16(�6e   $��0,10,0,0q�6s   5A      5B      5C      5D      5E      5F      