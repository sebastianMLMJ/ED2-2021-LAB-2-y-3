# ED2-2021-LAB-2
Repositorio de laboratorio 2 y 3  INSTRUCCIONES
Laboratorio_02
La rama main contiene el laboratorio numero 2 compresion de huffman, las rutas son las establecidas en el enunciado y la clave para enviar archivos a traves del cliente es File

Laboratorio_03
La rama LZW contiene al tercer laboratorio que implementa ambos algoritmos de compresión tanto huffman como lzw compressions sigue funcionando igual devuelve las compresione
tanto de huffman como las de lzw en un solo request.
Para la compresion de un archivo la ruta es api/compress/{huffman}/{name} para comprimir por medio de huffman y api/compress/{lzw}/{name} para comprimir con lzw donde el nombre es
el nombre con el que se retornara el archivo

la descompresion de un archivo es por la ruta api/decompress/{huffman} para descomprimir un archivo .huff y api/decompress/{lzw} para descomprimir un archivo .LZW
