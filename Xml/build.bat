@ECHO OFF
java -jar W:\xml\trang.jar -I xml -O xsd cescript.xml cescript.xsd
xsd /l:cs /c cescript.xsd