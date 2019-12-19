##Xaml Coding Guidelines

###The control attributes
- the first attribute is on the same code line
- each next attribute is on separate line
To achive this you can go to Visual Studio "Tools>Options...>Xaml>Formatting>Spacing" and choose the option "Position each attribute on separate line".

###Attributes order:
1. Control x:Name
2. Attributes with bindings, actions
3. Attributes referencing StaticResource.
4. Attributes defining the design and are set explicit.
5. Group attributes, that belongs together i.e. Width, Height or HorizontalAlignment, VerticalAlignment, putting them after each other.