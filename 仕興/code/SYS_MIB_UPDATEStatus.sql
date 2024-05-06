USE [DSMDBTEST]
GO
/****** Object:  StoredProcedure [dbo].[SYS_MIB_UPDATEStatus]    Script Date: 2024/5/6 下午 03:52:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[SYS_MIB_UPDATEStatus] (@PLine NVARCHAR(50),@MSN NVARCHAR(50))
AS	
	/*同步MIB & PIB 資料 */

	--修復StandardSP,BigModel,StandardWH
	--BEGIN
	DECLARE @COUNT_StandardSP INT = (SELECT COUNT(StandardSP) FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND StandardSP='null')
	DECLARE @COUNT_BigModel INT  = (SELECT COUNT(BigModel) FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND StandardSP='null')
	DECLARE @COUNT_StandardWH INT   = (SELECT COUNT(StandardWH) FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND StandardSP='null')
	
	IF @COUNT_StandardSP>0
		BEGIN
			UPDATE PIB SET StandardSP=NULL WHERE StandardSP='null' 
		END
	IF @COUNT_StandardWH>0
		BEGIN
			UPDATE PIB SET BigModel=NULL WHERE BigModel='null'
		END
	IF @COUNT_StandardWH>0
		BEGIN
			UPDATE PIB SET StandardWH=NULL WHERE StandardWH='null' 
		END
	--END

	
	
	/*同步PIB*/
	--<BEGIN>
	DECLARE @Status NVARCHAR(50) = (SELECT Status FROM MIB WHERE PLine=@PLine and MSN=@MSN)
	DECLARE @E1_Count		INT  = (SELECT COUNT(*) FROM MIB_E1 WHERE PLine=@PLine and MSN=@MSN)
	DECLARE @E1_Count_check INT  = (SELECT COUNT(*) from MIB_E1 WHERE PLine=@PLine and MSN=@MSN and FDate!='')
	DECLARE @PIB_Status NVARCHAR(10)

	--未完成處理	
	IF @Status='N'
		BEGIN
			IF @E1_Count>0
				BEGIN
					IF @E1_Count=@E1_Count_check
						BEGIN
							SET @PIB_Status = (SELECT Status FROM MIB WHERE PLine=@PLine and MSN=@MSN)
							IF @PIB_Status != 'V'
								BEGIN
									UPDATE MIB SET Status='V' WHERE PLine=@PLine and MSN=@MSN
								END
						END
					ELSE
						BEGIN							
							SET @PIB_Status = (SELECT Status FROM MIB WHERE PLine=@PLine and MSN=@MSN)
							IF @PIB_Status != 'N'
								BEGIN
									UPDATE MIB SET Status='N' WHERE PLine=@PLine and MSN=@MSN
								END
						END
				END
		END
    
	--完成處理
	IF @Status='V'
		BEGIN
			IF @E1_Count>0
				BEGIN
					IF @E1_Count=@E1_Count_check
						BEGIN
							IF @PIB_Status != 'V'
								BEGIN
									UPDATE MIB SET Status='V' WHERE PLine=@PLine and MSN=@MSN
								END
						END
					ELSE
						BEGIN
							IF @PIB_Status != 'N'
								BEGIN
									UPDATE MIB SET Status='N' WHERE PLine=@PLine and MSN=@MSN
								END
						END
				END
		END
	--<END>

	/*生產狀態判定優化,原生產狀態顯示待驗,但實際狀態還有其他工段尚未完成*/
	--<BEGIN>
	DECLARE @BigName NVARCHAR(100) = (SELECT BigName FROM MIB WHERE PLine=@PLine and MSN=@MSN)
	DECLARE @Model   NVARCHAR(50)  = (SELECT TOP 1 word FROM dbo.udf_Split(@BigName,' '))

	--彙集目前看板狀況
	--S,上線中
	--P,停工
	--E,待驗
	--Y,完工
	DECLARE @tmptable TABLE(
		ID  NVARCHAR(50),
		Status NVARCHAR(5)
	)
	DECLARE @SD001_Status NVARCHAR(1)= ''
	DECLARE @SD002_Status NVARCHAR(1)= ''
	DECLARE @SD003_Status NVARCHAR(1)= ''
	DECLARE @SD004_Status NVARCHAR(1)= ''
	DECLARE @SD005_Status NVARCHAR(1)= ''
	DECLARE @SD006_Status NVARCHAR(1)= ''
	DECLARE @SD007_Status NVARCHAR(1)= ''
	DECLARE @SD008_Status NVARCHAR(1)= ''

	--SD001,1,腳架作業
	DECLARE @SD001 FLOAT = (SELECT TOP 1 SD001 FROM StandardData WHERE Model LIKE @Model+'%' AND PLine=@PLine)
	IF @SD001>0
		BEGIN
			SET @SD001_Status = (SELECT TOP 1 Status FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND P_ID=1)
			INSERT INTO @tmptable(ID,Status)VALUES('腳架作業',@SD001_Status)
		END
	
	--SD002,2,洗台板作業
	DECLARE @SD002 FLOAT = (SELECT TOP 1 SD002 FROM StandardData WHERE Model LIKE @Model+'%' AND PLine=@PLine)
	IF @SD002>0
		BEGIN
			SET @SD002_Status = (SELECT TOP 1 Status FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND P_ID=2)
			INSERT INTO @tmptable(ID,Status)VALUES('洗台板作業',@SD002_Status)
		END
	
	--SD003,3,備料作業
	DECLARE @SD003 FLOAT = (SELECT TOP 1 SD003 FROM StandardData WHERE Model LIKE @Model+'%' AND PLine=@PLine)
	IF @SD003>0
		BEGIN
			SET @SD003_Status = (SELECT TOP 1 Status FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND P_ID=3)
			INSERT INTO @tmptable(ID,Status)VALUES('備料作業',@SD003_Status)
		END
	
	--SD004,4,副線作業
	DECLARE @SD004 FLOAT = (SELECT TOP 1 SD004 FROM StandardData where Model like @Model+'%' AND PLine=@PLine)
	IF @SD004>0
		BEGIN																	
			SET @SD004_Status = (SELECT TOP 1 Status FROM PIB WHERE PLine=@PLine and MSN=@MSN AND P_ID=4)
			INSERT INTO @tmptable(ID,Status)VALUES('副線作業',@SD004_Status)	
		END
	
	--SD005,5,走線作業
	DECLARE @SD005 FLOAT = (SELECT top 1 SD005 from StandardData where Model like @Model+'%' AND PLine=@PLine)
	IF @SD005>0
		BEGIN
			SET @SD005_Status = (SELECT TOP 1 Status from PIB where PLine=@PLine and MSN=@MSN AND P_ID=5)
			INSERT into @tmptable(ID,Status)VALUES('走線作業',@SD005_Status)
		END
	
	--SD006,6,線下作業
	DECLARE @SD006 FLOAT = (SELECT TOP 1 SD006 from StandardData where Model like @Model+'%' AND PLine=@PLine)
	IF @SD006>0
		BEGIN
			set @SD006_Status = (SELECT TOP 1 Status from PIB where PLine=@PLine and MSN=@MSN AND P_ID=6)
			INSERT into @tmptable(ID,Status)VALUES('線下作業',@SD006_Status)	
		END
	
	--SD007,7,封箱作業,不使用
	--DECLARE @SD007 FLOAT = (SELECT top 1 SD007 from StandardData where Model like @Model+'%' AND PLine=@PLine)
	--IF @SD007>0
	--	BEGIN
	--		set @SD007_Status = (SELECT top 1 Status from PIB where PLine=@PLine and MSN=@MSN AND P_ID=7)
	--		insert into @tmptable(ID,Status)VALUES('封箱作業',@SD007_Status)		
	--	END

	--SD008,8,佈線作業
	DECLARE @SD008 FLOAT = (SELECT TOP 1 SD008 from StandardData where Model like @Model+'%' AND PLine=@PLine)
	IF @SD008>0
		BEGIN
			SET @SD008_Status = (SELECT TOP 1 Status FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND P_ID=8)
			INSERT INTO @tmptable(ID,Status)VALUES('佈線作業',@SD008_Status)		
		END

	DECLARE @tmptable_count INT = (SELECT count(*) FROM @tmptable)
	DECLARE @tmptable_E INT = (SELECT count(*) FROM @tmptable where Status = 'E')
	DECLARE @tmptable_S INT = (SELECT count(*) FROM PIB where PLine=@PLine AND MSN=@MSN and Status = 'S')
	DECLARE @MIB_Pstatus NVARCHAR(10)
	
	--上線中狀態
	IF @tmptable_S>0
		BEGIN
			SET @MIB_Pstatus = (SELECT Pstatus FROM MIB WHERE PLine=@PLine AND MSN=@MSN)
			IF @MIB_Pstatus = 'S'
				BEGIN
					UPDATE MIB SET Pstatus='S' WHERE PLine=@PLine AND MSN=@MSN
				END
		END
	ELSE
	--未上線中狀態
		BEGIN
			--有停工狀態
			DECLARE @tmptable_P INT = (SELECT COUNT(*) FROM PIB WHERE PLine=@PLine AND MSN=@MSN AND Status = 'P')
			IF @tmptable_P>0
				BEGIN
					SET @MIB_Pstatus = (SELECT Pstatus FROM MIB WHERE PLine=@PLine AND MSN=@MSN)
					IF @MIB_Pstatus = 'P'
						BEGIN
							UPDATE MIB SET Pstatus='P' WHERE PLine=@PLine AND MSN=@MSN
						END
				END
			ELSE
			--沒有停工狀態
				BEGIN
					--全部待驗
					IF @tmptable_count=@tmptable_E
						BEGIN
							SET @MIB_Pstatus = (SELECT Pstatus FROM MIB WHERE PLine=@PLine AND MSN=@MSN)
							IF @MIB_Pstatus = 'E'
								BEGIN
									UPDATE MIB SET Pstatus='E' WHERE PLine=@PLine AND MSN=@MSN
								END
						END
					ELSE
					--沒有全部待驗
						BEGIN
							SET @MIB_Pstatus = (SELECT Pstatus FROM MIB WHERE PLine=@PLine AND MSN=@MSN)
							IF @MIB_Pstatus = NULL
								BEGIN
									UPDATE MIB SET Pstatus=NULL WHERE PLine=@PLine AND MSN=@MSN
								END
						END
				END
		END

	

	--未完成
	DECLARE @tmptable_NULL int = (SELECT COUNT(*) FROM @tmptable WHERE Status IS NULL AND Status NOT IN ('S','P'))
	IF @tmptable_NULL>0 AND @tmptable_S=0 AND @tmptable_P=0
		BEGIN
			SET @MIB_Pstatus = (SELECT Pstatus FROM MIB WHERE PLine=@PLine AND MSN=@MSN)
			IF @MIB_Pstatus = NULL
				BEGIN
					UPDATE MIB SET Pstatus=NULL WHERE PLine=@PLine AND MSN=@MSN
				END
		END
	--<END>

	/*合併完工*/
	--<BEGIN>
	/*
	DECLARE @COUNT INT = 1
	DECLARE MyCursor Cursor FOR
	SELECT CombinID FROM PIB WHERE PLine = @PLine AND MSN = @MSN AND Status='Y' AND flag='N'
	Open MyCursor 

	declare @CombinID varchar(50)
	Fetch NEXT FROM MyCursor INTO @CombinID
	While (@@FETCH_STATUS <> -1)
	BEGIN
		IF @CombinID IS NOT NULL
			BEGIN
				PRINT @CombinID
			END
		Fetch NEXT FROM MyCursor INTO @CombinID
	END
	CLOSE MyCursor
	DEALLOCATE MyCursor

	DECLARE @COUNT_UPDATE INT = 1
	DECLARE MyCursor_ Cursor FOR
	SELECT P_ID,Model,Amount,ISNULL(StandardWH,0) FROM PIB WHERE PLine = @PLine AND MSN = @MSN AND flag='Y'
	Open MyCursor_ 
	*/
	--<END>


	/*更新欄位*/
	--<BEGIN>
	/*
	DECLARE @P_ID varchar(50)
	DECLARE @Model_UPDATE varchar(50)
	DECLARE @Amount_UPDATE DECIMAL(18,2)
	DECLARE @StandardWH_UPDATE DECIMAL(18,2)
	Fetch NEXT FROM MyCursor_ INTO @P_ID,@Model_UPDATE,@Amount_UPDATE,@StandardWH_UPDATE
	While (@@FETCH_STATUS <> -1)
	BEGIN
		SET @Model = (SELECT TOP 1 * FROM udf_Split(@Model,' ') TB)
		IF @StandardWH = 0
			BEGIN
				SET @StandardWH_UPDATE = (SELECT CONVERT(DECIMAL(18,1),VAL/People*@Amount) FROM View_StandardDataLst WHERE Model=@Model AND P_ID=@P_ID)
				DECLARE @StandardSP_UPDATE DECIMAL(18,1)= @Amount/@StandardWH_UPDATE
				UPDATE PIB SET StandardWH=@StandardWH_UPDATE,StandardSP=@StandardSP_UPDATE WHERE PLine=@PLine AND MSN=@MSN AND P_ID=@P_ID
			END	
		Fetch NEXT FROM MyCursor_ INTO @P_ID,@Model,@Amount,@StandardWH
	END
	CLOSE MyCursor_
	DEALLOCATE MyCursor_
	*/
	--<END>
