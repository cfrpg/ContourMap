# ContourMap

二维等值线生成程序,使用Marching squares算法绘制三角形网格的等值线,需要预处理一下输入文件.

VS2015 Update3
MonoGame 3.6

# 输入文件

## 输入文件格式

每行包含一个数字

首先三个整数,点数Nodes,面数Elements,相邻面关系数FACENEIGHBORCONNECTIONS.之后依次输入点数据,面数据,相邻面关系数据.

点数据(Point格式):X,Y,V,分别表示X,Y坐标和该点的值.

面数据:四个顶点的索引(从1开始):P0,P1,P2,P2.注意Tecplot将三角形单元作为两点重合的四边形单元处理,第四个索引在读取时被忽略.

相邻面关系:c1,f,c2,表示c2与c1共用c1的第f条边,其中f为1,2或4.

## 生成输入文件

1. 使用Tecplot ASCII Data Writer,输出1个Zone,3个Variable,格式为Point;输出Field data和Generated face neighbor information.
2. 删除头部说明,只保留Nodes,Elements,FACENEIGHBORCONNECTIONS的三个数字.
3. 去除行首空格:将"换行符-空格"替换为"换行符".
4. 将数字变为换行分隔:将"空格"替换为"换行符".
