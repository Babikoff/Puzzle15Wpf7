   M S B A M L   `   `   `  ����  Puzzle15'    !Puzzle15.Wpf.GameFieldViewControl   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      � .+�5	   
   (�   K� ZIndex$  0��6   .��5
      ��  �� Orientation$ Vertical=�6   .+�5      �� $��10(�6   
��� ���$��Center=�6:   .��6^   Победа, ура!6l   �� 5      $��10(�6   
��� ���$��Center=�6:   .��6^   !Будет новая игра!6q   5      5      �� 5   
   -   5         ��Name$ GameAreaBorder��5   "   $  1��6   
����� �$��5q�6P    � 	Resources 5         BlinkStoryboard g� AutoReverse g� RepeatBehavior g� Duration g� AccelerationRatio g� 
SpeedRatio	 g� FillBehavior
 g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime O� Value��%�  &	        �� 5      -   5         . $ 1x��5   !   $ 	AutomaticD�65   $ 1Q�5   !   $ 3Q�67   $
	 Stop=�5   !   .��5      U� $
 00:00:00X�64   $ GameAreaBorder��6I   $ 	(Opacity)�6p   .\�5      �� $ 00:00:00��6/   $
 0.01Q�6B   �� 5      $ 
00:00:01.5��6/   $	 0.2Q�6D   �� 5      $ 
00:00:02.0��6/   $	 0.4Q�6D   �� 5      $ 
00:00:02.5��6/   $	 0.8Q�6D   �� 5      $ 
00:00:03.0��6/   $ 1Q�6D   5      5      5      ._�5      � -   5        $��ContentPanel��5      $��Center=�6,   $��Center=�6I   5      5       5!      