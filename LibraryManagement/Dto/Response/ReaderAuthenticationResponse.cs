namespace LibraryManagement.Dto.Response
{
    public class ReaderAuthenticationResponse
    {
        public string IdReader { get; set; }    
        
        public Guid IdTypeReader { get; set; }   
        
        public string? NameReader { get; set; }   
        
        public string? Sex { get; set; }           
        
        public string? Address { get; set; }        
        
        public string? Email { get; set; }         
        
        public DateTime Dob { get; set; }          
        
        public DateTime CreateDate { get; set; }      
        
        public DateTime ExpiryDate { get; set; }     
                
        public string RoleName { get; set; }    
        
        public string? TypeReader { get; set; }

        public string AvatarUrl { get; set; }
    }
}
