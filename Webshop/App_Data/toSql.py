# -*- coding: cp1252 -*-

class Product():
    id = 0
    name = None
    price = 0
    properties = {}
    textDescription = None
    imageName = None
    category = None
    subCat1 = None
    subCat2 = None


import xlrd

wb = xlrd.open_workbook('ProductsMini.xlsx')

s = wb.sheet_by_index(0)

data = {}
skipped = 0
print "Reading xlsx..."
# 0     1      2          3              4              5                6              7        8       9         10       11
#id | Name | Price | description1 | description2 | description3 | textDescription | ImageName | Cat | subCat1 | subCat2 | subCat3
#id | Name | Price | description1 | description2 | description3 | textDescription | ImageName | Cat | subCat1 | subCat2 | subCat3
for rowIndex in range(s.nrows):
    r = s.row(rowIndex)
    if r[2].value == "Price":
        continue
    c = None
    if data.has_key(r[1].value):
        c = data[r[1].value]
    else:
        c = Product()
        c.name = r[1].value
        c.price = int(float(r[2].value) * 100)
        try:
            c.textDescription = unicode(r[6].value.encode('ascii', 'ignore').replace("'", ""))
        except UnicodeEncodeError:
            skipped = skipped + 1
            continue
        c.imageName = r[7].value
        c.category = r[9].value
        c.subCat1 = r[10].value
        c.subCat2 = r[11].value
        data[r[1].value] = c

    if not c.properties.has_key(r[3].value):
        c.properties[r[3].value] = {}
    c.properties[r[3].value][r[4].value] = r[5].value

print "Done"
print "Skipped " + str(skipped) + " Because of unicode problems"
print "Writing products to file..."
f = open('insert.sql', 'w')
f.write("SET IDENTITY_INSERT [dbo].[Products] ON\n")
f.write("INSERT INTO [dbo].[Products] ([ProductID], [Name], [Price], [TextDescription], [ImageName], [Category], [SubCat1], [SubCat2]) VALUES \n")
id = 1
for product in data.values():
    f.write("(")
    f.write(str(id) + ", ")
    f.write("'" + product.name + "', ")
    f.write(str(product.price) + ", ")
    f.write("'" + product.textDescription + "', ")
    f.write("'" + product.imageName + "', ")
    f.write("'" + product.category + "', ")
    f.write("'" + product.subCat1 + "', ")
    f.write("'" + product.subCat2 + "'")
    f.write("),\n")
    id = id + 1

f.write("SET IDENTITY_INSERT [dbo].[Products] OFF")
f.close()
print "Done"

f.write("SET IDENTITY_INSERT [dbo].[Products] OFF")

print "Writing properties of products to file..."


#todo properties inserten :(

"""
for product in data.values():
    for properties1 in product.properties.keys():
        print properties1 + ":"
        for properties2 in product.properties[properties1].keys():
            if properties1 == properties2:
                print "\t" + product.properties[properties1][properties2]
            else:
                print "\t" + properties2 + ":" + product.properties[properties1][properties2]



    
    #print product.name + ": " + str(product.price)

    break

"""



