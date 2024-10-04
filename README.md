# Cipher
#### Scroll down for English version

Программа-аниматор семейства дисковых шифраторов.
## Управление
Диски шифратора поворачиваются путём перетаскивания их мышью с зажатой ЛКМ.  
При включённом автоматическом шифровании текст, введённый в левое поле, автоматически зашифровывается с помощью шифратора и, аналогично,
текст, введённый в правое поле, автоматически дешифруется.  
Шифратор настраивается с помощью пункта меню "Изменить шифратор". Для изменения пользовательского набора шифраторов следует изменить файл
config.csv рядом с исполняемым файлом. Первая строка файла - количество шифраторов, каждая последующая - их описание в формате:
- Название
- Материал (допустимые варианты: "Металл", "Дерево", "Бумага" и "Бронза")
- Возможно ли автоматическое шифрование (true/false)
- Количество дисков
- Алфавиты дисков

#  English
Program for Animating and Modeling a Disk Scrambler Family. Implemented as a term project during the academic year of 2018-2019.
## Controls
#### Warning: Russian is currently the only supported UI language
The scrambler disks are rotated by dragging them using mouse while holding LMB down.
With automatic encryption turned on the text typed in the field on the left is automatically encrypted using the scrambler and printed on the right. Similarly, the text typed in the field on the right is automatically decrypted.
The scrambler can be configured using menu entry "Изменить шифратор" (Change scrambler). To edit the custom scrambler set, one needs to edit config.csv file located next to the executable file. The first line of the file specifies the number of scramblers, while each of the following lines specifies their descriptions with following values separated by comma:
- Name of the scrambler
- Material (available options are : "Металл" (Metallic), "Дерево" (Wooden), "Бумага" (Paper) and "Бронза" (Bronze))
- Whether automatic encryption is supported (true/false)
- Number of disks
- Alphabets of the disks
