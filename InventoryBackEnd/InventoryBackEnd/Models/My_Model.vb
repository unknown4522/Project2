Public Class Campuses
    Public Property Campus_ID As String
    Public Property Campus_name As String

End Class
Public Class Brand_lIST
    Public Property Brand_ID As String
    Public Property Brand_name As String
    Public Property Campus_name As String
End Class
Public Class Employee_Account
    Public Property ID As String
    Public Property Firstname As String
    Public Property Lastname As String
    Public Property Campus_name As String
    Public Property Username As String
    Public Property Password As String
    Public Property Role As String
    Public Property ProfilePicturePath As String
End Class
Public Class Rooms
    Public Property ID As String
    Public Property Room_name As String
    Public Property Floor_level As String
    Public Property Room_description As String
    Public Property Campus_name As String
    Public Property Counter As String
End Class
Public Class Item_logs
    Public Property Item_name As String
    Public Property Serial_number As String
    Public Property Date_Transfer As String
    Public Property Last_location As String
    Public Property Current_location As String
    Public Property Transfer_by As String
    Public Property Requested_by As String
    Public Property Campus_name As String
    Public Property Process As String
    Public Property Model As String
    Public Property Brand As String
    Public Property Item_type As String
    Public Property item_status As String
    Public Property Department As String
End Class
Public Class Item_list
    Public Property ID As String
    Public Property Item_name As String
    Public Property Model As String
    Public Property Brand As String
    Public Property Serial_number As String
    Public Property Item_type As String
    Public Property Room_name As String
    Public Property Item_status As String
    Public Property Department As String
    Public Property Date_now As String
    Public Property Campus_name As String
End Class
Public Class Apparel_stock
    Public Property Apparel_ID As String
    Public Property Grade_level As String
    Public Property Apparel_type As String
    Public Property Size As String
    Public Property Quantity As String
    Public Property Transaction As String
    Public Property Campus_name As String
End Class
Public Class Grade_list
    Public Property ID As String
    Public Property Grade_level As String
    Public Property Campus_name As String
End Class

Public Class Apparel_type
    Public Property Apparel_ID As String
    Public Property Apparel_type As String
    Public Property Campus_name As String
End Class

Public Class Size_list
    Public Property Size_ID As String
    Public Property Size As String
    Public Property Campus_name As String
End Class

Public Class Apparel_logs
    Public Property ID As String
    Public Property Apparel_type As String
    Public Property Last_name As String
    Public Property First_name As String
    Public Property Grade_level As String
    Public Property Transaction As String
    Public Property Size As String
    Public Property Quantity As String
    Public Property Transaction_date As String
    Public Property Campus_name As String
    Public Property Reciept_number As String
End Class

Public Class Claim_stub
    Public Property Claim_ID As String
    Public Property Size As String
    Public Property Quantity As String
    Public Property Grade_level As String
    Public Property Apparel_name As String
    Public Property First_name As String
    Public Property Last_name As String
    Public Property Date_recieve As String
    Public Property Reciept_number As String
    Public Property Campus_name As String
End Class

Public Class Materials_list
    Public Property Material_ID As String
    Public Property Material_name As String
    Public Property Quantity As String
    Public Property Date_Purchased As String
    Public Property Received_date As String
    Public Property Request_by As String
    Public Property Approve_by As String
    Public Property Status As String
    Public Property Campus_name As String
    Public Property Supplier As String
    Public Property Unit As String
    Public Property Received_By As String
End Class

Public Class Fieldcounter
    Public Property Counter As String
End Class
