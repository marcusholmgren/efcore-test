# Unexpected delete statement generated

This repository demonstrates a unexpected delete statment when saving changes on a loaded entity in a new session.

See `dotnet_run.log`for the output. From line 216 the delete statement is shown.


# SQL status

```
select * from Blogger.Blogs
  left join dbo.Posts on Blogs.BlogId = Posts.BlogId
```

<table border="1" style="border-collapse:collapse">
<tr><th>BlogId</th><th>Url</th><th>PostId</th><th>BlogId</th><th>Content</th><th>Title</th></tr>
<tr><td>1</td><td>http://blogs.msdn.com/adonet</td><td>2</td><td>1</td><td>I did not expected this...</td><td>Unexpected delete</td></tr></table>
