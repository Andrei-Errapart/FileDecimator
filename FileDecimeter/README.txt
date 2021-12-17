Usage
-----


FileDecimeter.exe INPUT_FILE OUTPUT_FILE

The settings are read from the initialization file "FileDecimeter.ini"

Input file format: separated by ";", tabs or spaces. When decimal separator is not comma (,), the comma is added to the separators as well.

Output file format: Separated by the separator specified in the initialization file.

The numbers are assumed to be in the local regional format.


Initialization file format
--------------------------


Windows INI-file

Name        | Description
------------+---------------
Dot         | Decimal separator in numbers.
Column      | Column index, starting from 1.
Negative    | Invert the values when "True"
Separator   | Separator character, tab for the tabulator and space for space.


Default initialization file
---------------------------
Dot=,
Column=3
Negative=False
Separator=tab




