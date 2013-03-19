class Product():
    def __init__(self, id):
        self.id = id
        self.name = None
        self.price = 0
        self.properties = []
        self.textDescription = None
        self.imageName = None
        self.category = None
        self.subCat1 = None
        self.subCat2 = None


import xlrd
import random
from datetime import datetime


print "Reading xlsx..."
startTime = datetime.now()
wb = xlrd.open_workbook('Products.xlsx')
print "Done (" + str((datetime.now() - startTime).seconds) + " seconds)"
s = wb.sheet_by_index(0)

data = {}
skipped = 0
id = 1
lastProductName = ""
skipId = 0
toTake = 4 #skip (toTake - 1) out of toTake
choices = range(toTake)
print "Parsing products..."
startTime = datetime.now()

# 0     1      2          3              4              5                6              7        8       9         10       11
#id | Name | Price | description1 | description2 | description3 | textDescription | ImageName | Cat | subCat1 | subCat2 | subCat3
#id | Name | Price | description1 | description2 | description3 | textDescription | ImageName | Cat | subCat1 | subCat2 | subCat3
for rowIndex in range(s.nrows):
    r = s.row(rowIndex)
    if r[2].value == "Price":
        continue
    #if skipId == int(r[0].value) or random.choice(choices) != 1:
    #    skipId = int(r[0].value)
    #    continue
    c = None
    if data.has_key(r[1].value):
        c = data[r[1].value]
    else:
        c = Product(id)
        c.name = r[1].value.replace("'", "").replace('"', "")
        c.price = int(float(r[2].value) * 100)
        try:
            c.textDescription = r[6].value.replace("'", "").replace('"', "")
        except UnicodeEncodeError:
            skipped = skipped + 1
            continue
        c.imageName = r[7].value
        c.category = r[9].value.replace("'", "").replace('"', "")
        c.subCat1 = r[10].value.replace("'", "").replace('"', "")
        c.subCat2 = r[11].value.replace("'", "").replace('"', "")
        data[r[1].value] = c
        id = id + 1

    c.properties.append(r[3].value.replace("'", "").replace('"', "") + ":;:" + r[4].value.replace("'", "").replace('"', "") + ":;:" + r[5].value.replace("'", "").replace('"', ""))


print "Done (" + str((datetime.now() - startTime).seconds) + " seconds)"
print "Parsed " + str(len(data)) + " products"

print "Sorting products based on id..."
startTime = datetime.now()
data = sorted(data.values(), key=lambda product: product.id)
print "Done (" + str((datetime.now() - startTime).seconds) + " seconds)"

print "Writing products to file..."
startTime = datetime.now()
f = open('insertBig.sql', 'w')
f.write("SET IDENTITY_INSERT [dbo].[Products] ON\n")
f.write("INSERT INTO [dbo].[Products] ([ProductID], [Name], [Price], [TextDescription], [ImageName], [Category], [SubCat1], [SubCat2]) VALUES \n")
id = 1
values = data
for product in values:
    if id % 999 == 0:
        f.write("INSERT INTO [dbo].[Products] ([ProductID], [Name], [Price], [TextDescription], [ImageName], [Category], [SubCat1], [SubCat2]) \n")
        f.write("VALUES\n")
    f.write("(")
    f.write(str(product.id) + ", ")
    f.write("'" + product.name.encode('utf-8', 'ignore') + "', ")
    f.write(str(product.price) + ", ")
    f.write("'" + product.textDescription.encode('utf-8', 'ignore') + "', ")
    f.write("'" + product.imageName.encode('utf-8', 'ignore') + "', ")
    f.write("'" + product.category.encode('utf-8', 'ignore') + "', ")
    f.write("'" + product.subCat1.encode('utf-8', 'ignore') + "', ")
    f.write("'" + product.subCat2.encode('utf-8', 'ignore') + "'")
    f.write(")")
    id = id + 1
    if id <= len(values) and id % 999 != 0:
        f.write(",\n")
    else:
        f.write("\n")

f.write("SET IDENTITY_INSERT [dbo].[Products] OFF\n")

print "Done (" + str((datetime.now() - startTime).seconds) + " seconds)"

f.write("SET IDENTITY_INSERT [dbo].[ProductProperties] ON\n")
f.write("INSERT INTO [dbo].[ProductProperties] ([ProductPropertyID], [ProductID], [Property1], [Property2], [Property3])\n")
f.write("VALUES\n")
print "Writing properties of products to file..."
startTime = datetime.now()

#todo properties inserten :(
id = 1
for product in values:
    for propertieString in product.properties:
        values = propertieString.split(":;:")
        if values[0] is "":
            continue
        if id % 999 == 0:
            f.write("INSERT INTO [dbo].[ProductProperties] ([ProductPropertyID], [ProductID], [Property1], [Property2], [Property3])\n")
            f.write("VALUES\n")
        f.write("(")
        f.write(str(id) + ", ")
        f.write(str(product.id) + ", ")
        f.write("'" + values[0].encode('utf-8', 'ignore') + "', ")
        f.write("'" + values[1].encode('utf-8', 'ignore') + "', ")
        f.write("'" + values[2].encode('utf-8', 'ignore') + "'")
        f.write(")")
        id = id + 1
        if id % 999 == 0:
            f.write("\n")
        else:
           f.write(",\n")

f.write("SET IDENTITY_INSERT [dbo].[ProductProperties] OFF\n")
print "Done (" + str((datetime.now() - startTime).seconds) + " seconds)"
print "All Done"
f.close()


