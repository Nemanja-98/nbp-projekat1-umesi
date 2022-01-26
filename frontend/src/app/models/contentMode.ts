export class ContentMode{
    static CreatePost = new ContentMode("createPost");
    static Post = new ContentMode("post")
    static FavouritePost = new ContentMode("favouritePost")
    static FollowedUser = new ContentMode("followedUser")
    displayMode: string;
  
    constructor(displayMode :string){
      this.displayMode = displayMode;
    }
}  