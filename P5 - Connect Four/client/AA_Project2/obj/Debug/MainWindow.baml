   M S B A M L   `   `   `  ����F  BAA_Project2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:AA_Project2AA_Project2  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	       AA_Project2.MainWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   +localclr-namespace:AA_Project2     5   	   9System&clr-namespace:System;assembly=mscorlib     5   	     � Title$  
MainWindow��5	   	   $	��488��6   $��699.333��6)    � WindowStartupLocation$ CenterScreen=�5
   	   .��5      � $��0,0,-6.667,-0.333q�6   .+�5   
   �� -   5         ��Name$ btn_Connect��5   \   $��Connect��6   $��32��6#   $
��Left=�6/   $��40,3,0,0q�6J   $	��Top=�6q   $��76��6�   #��� ��6�   
���   �6�   �� 5   
   -   5         ��Name$ listBox1��5   T   $	��101��6   $
��Left=�6   $��459,275,-1.667,0q�6:   $	��Top=�6f   $	��222��6~   6�   �� 5   
   -   5        $ btn_Send��5   a   $
��Send��6   $��35��6    $
��Left=�6,   $��648,386,-6.667,0q�6G   $	��Top=�6s   $��38��6�    J� RenderTransformOrigin$ 5.257,-0.197&�6�   �� 5   
   -   5         ��Name$ textBox1��5   N   $��35��6   $
��Left=�6   $��458,386,0,0q�69   $	��Top=�6`   $	��185��6x   6�   �� 5   
   -   5        $ btn_Connect_Copy��5   `   $��
Disconnect��6   $��32��6&   $
��Left=�62   $��	116,3,0,0q�6M   $	��Top=�6z   $��75��6�   6�   o� 5   
   $
��Left=�6   $��32��6*   $	��Top=�66   $��42��6N   $��-2,3,0,0q�6Y   .@�5      m� -   5         m�Name$ mi_file��5      $��_File��6(   $��32��67   $��42��6C   ���	#FFDDDDDD#��� ��6e   .@�5      m� -   5        $��_Exit��5      6*   �� 5      m� 5      $��Instructions��6   .@�5      m� -   5        $	 One��5      $)��#1. Make sure the server has started��6,   $��	0,0,-80,0q�6Y   m� 5      -	   5        $	 two��5      $��2. Click on 'connect' ��6,   m� 5      -
   5        $ three��5      $��3. Enter a username��6.   m� 5      -   5        $
 four��5      $B��<4. Use the left and right buttons to move the ball sideways ��6-   m� 5      -   5        $
 five��5      $?��95. Click Drop once decided on the column to drop the ball��6-   m� 5      -   5        $	 six��5      $<��66. You can use the chat field to talk to other players��6,   m� 5      -   5        $ seven��5      $��7. Have fun!��6.   5      �� 5       m� 5!      $��_About��6   .@�5"      m� -   5        $ About��5"      $����Author: Abdullah Aljandali                       
Date: 11/7/2018  
Course: EE 356  
Assignment: Project 5b                  

This is the client side of a multiplier network application of the connect four game.
You compete with another player across the network.  ��6.   $	��111��6Z  $��0,0,-15.667,0q�6g  5$      5%      5&      �� 5'   
   -   5         ��Name$
 cnv1��5'      $
��Left=�6   $	��314��6:   $��
13,101,0,0q�6G   $	��Top=�6[   $	��408��6s   #��� ��6   �� 5(   
   $��Chat:��6   $
��Left=�6    $��459,249,0,0q�6;   $	��Top=�6P   �� 5)   
   $��Players waiting:��6   $
��Left=�6+   $��459,133,0,0q�6F   $	��Top=�6[   �� 5*   
   -   5        $ lst_waiting��5*      $
��Left=�6'   $��75��6B   $	��Top=�6N   $	��222��6f   $��459,164,-1.667,0q�6r   �� 5+   
   -   5         ��Name$ lbl_player1��5+      $��Player1��6%   $
��Left=�67   $��
486,55,0,0q�6R   $	��Top=�6f   �� 5,   
   -   5        $ lbl_player2��5,      $��Player2��6%   $
��Left=�67   $��
595,55,0,0q�6R   $	��Top=�6f   A� 5-   
   -   5        	 A�Name$	 e1��5-      ���	#FFF4F4F5$
��Left=�60   $��47��6K   $��
490,86,0,0q�6W   
���   �$	��Top=�6z   $��45��6�   A� 5.   
   -   5        $	 e2��5.      ���	#FFF4F4F5$
��Left=�60   $��47��6K   $��
595,86,0,0q�6W   
���   �$	��Top=�6z   $��45��6�   $ -2.956,0.681&�6�   A� 5/   
   -   5        $
	 curr��5/      ���	#FFF4F4F5$
��Left=�62   $��41��6M   $��	23,55,0,0q�6Y   
���   �$	��Top=�6{   $��40��6�   �� 50   
   -   5        $
 drop��50      $
��Drop��6   $
��Left=�6.   $��
290,20,0,0q�6I   $	��Top=�6]   $��47��6u   6�   �� 51   
   -   5        $ right��51      $��Right��6    $
��Left=�60   $��
342,11,0,0q�6K   $	��Top=�6_   $��75��6w   6�   �� 52   
   -   5        $
 left��52      $
��left��6   $
��Left=�6.   $��
210,11,0,0q�6I   $	��Top=�6]   $��75��6u   6�   �� 53   
   $��	Username:��6   $
��Left=�6$   $��
487,11,0,0q�6?   $	��Top=�6S   �� 54   
   -   5        $ lbl_username��54      $
��User��6&   $
��Left=�65   $��
573,13,0,0q�6P   $	��Top=�6d   55      56      