USE [DSMDBTEST]
GO
/****** Object:  StoredProcedure [dbo].[SYS_PIB_CombinMSM]    Script Date: 2024/5/6 �U�� 04:36:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SYS_PIB_CombinMSM] (@CombinID NVARCHAR(50))
AS	
	/*���o�s�W�X�ָ��*/
	--���oCombinID������
	DECLARE @CombinDetail INT= (SELECT COUNT(*) FROM PIB WHERE flag='Y' AND MSN IS NULL AND CombinID=@CombinID)
	IF @CombinDetail = 0
	BEGIN
		--���o�s�W���
		--PLine			-> �[�u�t��
		--MSN			-> ���o�s�O�渹,�渹��渹�����[<br>
		--SDate			-> ���i���g
		--Model			-> ���o�Ҩ�,�Ҩ��Ҩ㤤���[<br>
		--Amount 		-> �֥[�ƶq
		--P_ID 			-> �u�q,�ۦP�u�q
		--StandardWH 	-> �֥[�X�֮ɶ�
		--flag 			-> 'Y'
		
		DECLARE @PLine NVARCHAR(50)=(SELECT TOP 1 PLine FROM PIB WHERE flag='Y' AND CombinID=@CombinID ORDER BY 1)
		--2024.05.06 �վ�50->MAX
		DECLARE @MSN NVARCHAR(MAX)=(SELECT RTRIM(MSN)+',' FROM PIB WHERE flag='Y' AND CombinID=@CombinID FOR XML PATH(''))
		SET @MSN = REPLACE(REVERSE(STUFF(REVERSE(@MSN),1,1,'')),',',N'<br>')
		
		DECLARE @SDate NVARCHAR(50)=(SELECT MIN(SDate) FROM PIB WHERE flag='Y' AND CombinID=@CombinID )
		DECLARE @EDate NVARCHAR(50)=(SELECT MAX(EDate) FROM PIB WHERE flag='Y' AND CombinID=@CombinID )
		--2024.05.06 �վ�50->MAX
		DECLARE @Model NVARCHAR(MAX)=(SELECT RTRIM(Model)+',' FROM PIB WHERE flag='Y' AND CombinID=@CombinID FOR XML PATH(''))
		SET @Model = REPLACE(REVERSE(STUFF(REVERSE(@Model),1,1,'')),',',N'<br>')
		
		DECLARE @Amount NVARCHAR(50)=(SELECT SUM(Amount) FROM PIB WHERE flag='Y' AND CombinID=@CombinID)
		DECLARE @StandardWH nchar(10)=(SELECT SUM(CONVERT(float,ISNULL(StandardWH,'0'))) FROM PIB WHERE flag='Y' AND CombinID=@CombinID)
		DECLARE @P_ID NVARCHAR(50)=(SELECT TOP 1 P_ID FROM PIB WHERE flag='Y' AND CombinID=@CombinID)
		
		--�g�J�s�W�X�ָ��
		INSERT INTO PIB (PLine,MSN,SDate,Model,Amount,P_ID,StandardWH,CombinID,flag,Ptime,PT001,PT002,PN001,PN002)VALUES(@PLine,@MSN,@SDate,@Model,@Amount,@P_ID,@StandardWH,@CombinID,'Y',0,GETDATE(),GETDATE(),0,0)
	END
