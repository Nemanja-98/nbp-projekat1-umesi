export class User {
    public Name : string;
    public Surname : string;
    public Username : string;
    public Password : string;

    constructor(Name :string, Surname :string, Username :string, Password: string){
        this.Name = Name;
        this.Surname = Surname;
        this.Username = Username;
        this.Password = Password;
    }
}
