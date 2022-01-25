import { Post } from "./post";

export class User {
    public name : string;
    public surname : string;
    public username : string;
    public password : string;
    public createdRecipes: Post[];
    public favoriteRecipes: Post[];
    public followedUsers: string[];

    constructor(Name :string, Surname :string, Username :string, Password: string, CreatedRecipes: Post[], FavoriteRecipes: Post[], FollowedUsers: string[]){
        this.name = Name;
        this.surname = Surname;
        this.username = Username;
        this.password = Password;
        this.createdRecipes = CreatedRecipes;
        this.favoriteRecipes = FavoriteRecipes;
        this.followedUsers = this.followedUsers;
    }
}
