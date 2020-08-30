if(typeof ($jskey) != "object"){$jskey = {};}
$jskey.menu =
{
num:1,
list:[],
hidden:true,//隐藏所有菜单
load:function(index, id){},//异步加载菜单
root:"",
path:"",
imgPath:"",
$:function(id){return document.getElementById(id);},
//初始化操作
reset:function(){this.list = [];},
//添加一级菜单下的一组子菜单的菜单项
put:function(i, title, id, html){
	this.list[this.list.length] = {"index":i, "title":title, "id":id, "html":html};//index位置索引,id一级菜单的id,title一级菜单的name
},
//创建一层菜单内容
createCell:function(obj, c){
	var html = '<tr>';
	html += '<td id="JskeyMT_' + c + '" class="menuClose" onclick="$jskey.menu.clickBar(' + c + ')"' + ((c == this.list.length)?' style="display:none;"':'') + '>';
	html += '<img id="JskeyI' + 'JskeyMT_' + c + '" align="absmiddle" style="border:0;" src="' + this.imgPath + 'right.gif" /> ';
	html += obj.title + '</td>';
	html += '</tr><tr>';
	html += '<td id="JskeyMC_' + c + '" valign="top" class="cellClose" style="' + ((c == this.list.length)?'display:;height:100%':'display:none;height:') + '">';
	html += '<div class="cellContent" id="JskeyMCD_' + c + '">';
	html += ((obj.html != "")?obj.html:' ');
	html += '</div></td></tr>';
	return html;
},
//创建一层菜单内容
create:function(){
	var len = this.list.length;
	if(len < 1){return;}
	var html = '<table id="JskeyMG" class="jskeymenu" cellspacing="0" cellpadding="0">';
	for(var c = 0;c < len;c++){html += this.createCell(this.list[c], c);}
	html += this.createCell({"index":len, "title":"", "id":"", "html":""}, len);// 在最后补一个看不见的，用于收缩
	html += '</table>';
	try{if(this.$("JskeyMG") != null){document.body.removeChild(this.$("JskeyMG"));}}catch(e){}
	document.body.innerHTML += html;
},
//滑动菜单
smoothMenu:function(cOpen, cClose, stat){
	var timeDelay = 20;
	var moveHight = 20;
	if(stat == 0){
		this.$("JskeyMC_" + cOpen).style.display = "";
		setTimeout("$jskey.menu.smoothMenu(" + cOpen + "," + cClose + "," + moveHight + ")", timeDelay);
	}
	else{
		if(stat > 100){stat = 100;}
		this.$("JskeyMC_" + cOpen).style.height = stat + "%";
		this.$("JskeyMC_" + cClose).style.height = (100 - stat) + "%";
		if(stat < 100){
			stat += moveHight;
			setTimeout("$jskey.menu.smoothMenu(" + cOpen + "," + cClose + "," + stat + ")", timeDelay);
		}
		else{
			this.$("JskeyMC_" + cClose).style.display = "none";
		}
	}
},
//判断点了对应的菜单块
clickBar:function(c){
	if(c >= 0){
		try{this.click(c, this.list[c].id);this.load(c, this.list[c].id);}catch(e){}
		var isClose = this.$("JskeyMC_" + c).style.display == "";
		if(isClose){
			this.$("JskeyMT_" + c).className = "menuClose";
			this.$("JskeyMC_" + c).className = "cellClose";
			this.$("JskeyI" + "JskeyMT_" + c).src = this.imgPath + "right.gif";
			if(this.hidden){
				this.$("JskeyMC_" + this.list.length).style.display = "";
				this.smoothMenu(this.list.length, c, 0);// 需要将最后一个展开
			}
			else{
				this.$("JskeyMC_" + c).style.display = "none";
			}
		}
		else{
			this.$("JskeyMT_" + c).className = "menuOpen";
			this.$("JskeyMC_" + c).className = "cellOpen";
			this.$("JskeyI" + "JskeyMT_" + c).src = this.imgPath + "down.gif";
			if(this.hidden){
				for(var i = 0;i < this.list.length;i++){
					if(i != c && this.$("JskeyMC_" + i).style.display == ""){// 找到有其他被打开的就直接关掉并返回
						this.$("JskeyMT_" + i).className = "menuClose";
						this.$("JskeyMC_" + i).className = "cellClose";
						this.$("JskeyI" + "JskeyMT_" + i).src = this.imgPath + "right.gif";
						this.smoothMenu(c, i, 0);
						return;
					}
				}
				this.smoothMenu(c, this.list.length, 0);// 找不到被打开的，就关掉最后一个
			}
			else{
				this.$("JskeyMC_" + c).style.display = "";
			}
		}
	}
},
//用于异步回调，或刷新指定层次菜单
showNode:function(index, data){
	try{this.showNodeHTML(index, $jskey.menu.getNodeHtml({name:this.list[index].title, items:data}));}catch(e){};
},
showNodeHTML:function(index, html){
	try{this.list[index].html = html;this.$("JskeyMCD_" + index).innerHTML = html;}catch(e){};
},
// 单击
click:function(index, id){
},
//鼠标移过
imgMouse:function(obj, v, imgpath, over){
	if(over){
		obj.className = "imgdiv overfont";
		this.$("JskeyI" + v).src = imgpath;
		this.$("JskeyI" + v).className = "imgover";
	}
	else{
		obj.className = "imgdiv outfont";
		this.$("JskeyI" + v).src = imgpath;
		this.$("JskeyI" + v).className = "imgout";
	}
},
//树父级节点
expandNode:function(imgid, oid, img, imgOpen){
	if(this.$(oid).style.display == 'none'){
		this.$(oid).style.display = '';
		this.$(imgid).src = this.imgPath + "jian.gif";
		this.$(imgid + "_node").src = imgOpen;
	}
	else{
		this.$(oid).style.display = 'none';
		this.$(imgid).src = this.imgPath + "jia.gif";
		this.$(imgid + "_node").src = img;
	}
},
//取得一个菜单单元HTML代码
getCellHTML:function(obj, pnodeName){
	var html = "";
	var v = this.num++;
	if(obj.items.length == 0){
		var _img = this.path + ((obj.img == null || obj.img == "") ? "default.gif" : obj.img);
		var _imgOpen = this.path + ((obj.imgOpen == null || obj.imgOpen == "") ? "default.gif" : obj.imgOpen);
		html += "<div style=\"text-align:center;margin:auto;\">";
		html += "<div class=\"imgdiv outfont\" onmouseover=\"$jskey.menu.imgMouse(this,'" + v + "','" + _imgOpen + "',true)\"";
		html += "\tonmouseout=\"$jskey.menu.imgMouse(this, '"+v+"', '" + _img + "',false)\"";
		html += " ondblclick=\"$jskey.menu.reChangeURL('" + pnodeName + "','" + obj.name + "','" + obj.url + "')\" onclick=\"$jskey.menu.changeURL('" + pnodeName + "','" + obj.name + "','" + obj.url + "')\">";
		html += "<img id=\"JskeyI"+v+"\"  class=\"imgOut\" src=\"" + _img + "\"/>";
		html += obj.name + "</div></div>";//"<br/>" + 
	}
	return html;
},
//icoString:0和1的字符串
getTreeHtml:function(obj, pnodeName, icoString){
	var html = "";
	var v = this.num++;
	var len = icoString.length;
	var items = obj.items;
	if(icoString.length == 1 && items.length == 0){
		var _timg = this.path + ((obj.img == null || obj.img == "") ? "default.gif" : obj.img);
		html += "<div class='treenode' ondblclick=\"$jskey.menu.reChangeURL('" + pnodeName + "','" + obj.name + "','" + obj.url + "')\" onclick=\"$jskey.menu.changeURL('" + pnodeName + "','" + obj.name + "','" + obj.url + "')\">";
		html += "<img style='border:0px;' align='absmiddle' src='" + this.imgPath + ((icoString.charAt(len - 1) == '1') ? "L" : "T") + ".gif' />";
		html += "<img class='img' align='absmiddle' src='" + _timg + "' />";
		html += "<div class='treenodeout' onmouseover='this.className = \"treenodeover\"' onmouseout='this.className = \"treenodeout\"'>" + obj.name + "</div>";
		html += "</div>";
	}
	else{
		var tmpHTML = "";//保存子节点HTML
		// 父节点的图标是否由json来决定
		var _img = ((obj.img == null || obj.img == "")?(this.imgPath + "close.gif"):(this.path + obj.img));
		var _imgOpen = ((obj.imgOpen == null || obj.imgOpen == "")?(this.imgPath + "open.gif"):(this.path + obj.imgOpen));
		html += "<div class='treenode'" + ">";
		// 处理上级节点
		for(var i = 0;i < len - 1;i++){
			html += "<img style='border:0px;' align='absmiddle' src='" + this.imgPath + ((icoString.charAt(i) == '0') ? "I" : "N") + ".gif' />";
		}
		html += "<img id='JskeyI"+v+"' align='absmiddle' src='" + this.imgPath + "jian.gif' onclick='$jskey.menu.expandNode(\"JskeyI"+v+"\", \"DIV"+v+"\", \"" + _img + "\", \"" + _imgOpen + "\");' />";
		html += "<img id='JskeyI"+v+"_node' class='img' align='absmiddle' src='" + _imgOpen + "' onclick='$jskey.menu.expandNode(\"JskeyI"+v+"\", \"DIV"+v+"\", \"" + _img + "\", \"" + _imgOpen + "\");'/>";
		html += "<div class='treedivout' onmouseover='this.className = \"treedivover\"' onmouseout='this.className = \"treedivout\"' onclick='$jskey.menu.expandNode(\"JskeyI"+v+"\", \"DIV"+v+"\", \"" + _img + "\", \"" + _imgOpen + "\");'>" + obj.name + "</div>";
		html += "</div>";
		html += "<div class='treenodes' id='DIV"+v+"' style='display:;'>";
		var last = items.length - 1;
		var item;
		for(var i = 0;i < items.length;i++){
			item = items[i];
			if(item.items.length == 0){
				var _timg = this.path + ((item.img == null || item.img == "") ? "default.gif" : item.img);
				tmpHTML += "<div class='treenode' ondblclick=\"$jskey.menu.reChangeURL('" + pnodeName + "','" + item.name + "','" + item.url + "')\" onclick=\"$jskey.menu.changeURL('" + pnodeName + "','" + item.name + "','" + item.url + "')\">";
				for(var c = 0;c < len;c++){
					tmpHTML += "<img style='border:0px;' align='absmiddle' src='" + this.imgPath + ((icoString.charAt(c) == '0') ? "I" : "N") + ".gif' />";
				}
				tmpHTML += "<img style='border:0px;' align='absmiddle' src='" + this.imgPath + ((i == last) ? "L" : "T") + ".gif' />";
				tmpHTML += "<img id='JskeyI" + item.id + "_node' class='img' align='absmiddle' src='" + _timg + "'/>";
				tmpHTML += "<div class='treenodeout' onmouseover='this.className = \"treenodeover\"' onmouseout='this.className = \"treenodeout\"'>" + item.name + "</div>";
				tmpHTML += "</div>";
			}
			else{
				var icoStr = icoString + ((i == last) ? "1" : "0");
				tmpHTML += this.getTreeHtml(item, pnodeName, icoStr);//这里的pnodeName不变成name
			}
		}
		html += tmpHTML;
		html += "</div>";
	}
	return html;
},
//获得OutLook菜单子菜单中的html代码
getNodeHtml:function(obj){//{name:"",items:[]}
	var html = "";
	var items = obj.items;
	var childrenHTML = "";//保存功能节点HTML
	var item;
	var isTree = false;
	for(var i = 0;i < items.length;i++){
		//子节点，无下级节点
		if(items[i].items.length > 0){
			isTree = true;
			break;
		}
	}
	if(isTree){
		for(var i = 0;i < items.length;i++){
			item = items[i];
			childrenHTML += this.getTreeHtml(item, obj.name, ((i == items.length-1) ? "1" : "0"));
			// 增加此功能树节点
			html += childrenHTML;
			childrenHTML = "";// 清空
		}
	}
	else{
		for(var i = 0;i < items.length;i++){
			item = items[i];
			childrenHTML += this.getCellHTML(item, obj.name);
			//增加此功能节点
			html += childrenHTML;
			childrenHTML = "";
		}
	}
	return html;
}

};

$jskey.menu.Map = function(){this.data = [];};
$jskey.menu.Map.prototype ={
	put:function(key, value){if(key.length > 0){for(var i = 0; i < this.data.length; i++){if(key == this.data[i][0]){this.data[i][1] = value;return;}}this.data[this.data.length] = [key, value];}},
	get:function(key){if(key.length > 0){for(var i = 0; i < this.data.length; i++){if(key == this.data[i][0]){return this.data[i][1];}}}return null;}
};
$jskey.menu.format = function(){
	try{
		var data = arguments[0]||[];
		var v = arguments[1]||"0";
		var m = new $jskey.menu.Map(),root = [],po;
		for(var i = 0; i < data.length; i++){
			data[i].img = (data[i].img == null)?"":data[i].img;
			data[i].imgOpen = (data[i].imgOpen == null)?"":data[i].imgOpen;
			data[i].items = [];
			m.put(data[i].id + "", data[i]);// 把obj放入map
		}
		for(var i = 0; i < data.length; i++){
			if(data[i].pid != v){
				po = m.get(data[i].pid + "");
				if(po != null){
					var len = po.items.length;
					po.items[len] = data[i];// 把子节点放入父节点的items数组中
				}
			}
			else{root[root.length] = data[i];}
		}
		return root;
	}catch(e){alert(e.message);return [];}
};

$jskey.menu.jsSrc = "" + document.getElementsByTagName("script")[document.getElementsByTagName("script").length - 1].src;
//当前js的引用路径
$jskey.menu.jsPath = $jskey.menu.jsSrc.substring(0, $jskey.menu.jsSrc.lastIndexOf("/"));

$jskey.menu.show = function(items, isHidden){
	$jskey.menu.hidden = (isHidden) ? true : false;
	if($jskey.menu.path == "") $jskey.menu.path = $jskey.menu.jsPath + "/themes/menu/ico/";
	if($jskey.menu.imgPath == "") $jskey.menu.imgPath = $jskey.menu.jsPath + "/themes/menu/img/";
	$jskey.menu.reset();
	for(var i = 0;i < items.length;i++){
		var item = items[i];
		var html = $jskey.menu.getNodeHtml(item);
		$jskey.menu.put(i, item.name, item.id, html);
	}
	$jskey.menu.create();
};
//改变URL，可以覆盖此方法，用于自定义不同的方式
$jskey.menu.changeURL = function(parentname, nodename, url){
	if(url == null || url == "" || url == "null"){url = "";}
	if($jskey.menu.root != ""){
		if(url.indexOf("http") != 0 && url.indexOf($jskey.menu.root) != 0){
			url = $jskey.menu.root + url;
		}
	}
	if(url != ""){try{
		var s = nodename;//parentname + '-'+nodename;
		if(parent.$('#tt').tabs('exists', s)){
			parent.$('#tt').tabs('select', s);
		}
		else{
			parent.$('#tt').tabs('add',{
				title:s,
				content:'<div style="overflow:hidden;width:100%;height:100%;padding:0px;margin:0px;"><iframe scrolling="yes" frameborder="0" src="' + url + '"></iframe></div>',
				closable:true
			});
		}
	}catch(e){}}
};
$jskey.menu.reChangeURL = function(parentname, nodename, url){
	if(url == null || url == "" || url == "null"){url = "";}
	if($jskey.menu.root != ""){
		if(url.indexOf("http") != 0 && url.indexOf($jskey.menu.root) != 0){
			url = $jskey.menu.root + url;
		}
	}
	if(url != ""){try{
		var s = nodename;//parentname + '-'+nodename;
		if(parent.$('#tt').tabs('exists', s)){
			parent.$('#tt').tabs('select', s);
			parent.$('#tt').tabs('getTab', s).find('iframe')[0].src = url;
		}
		else{
			parent.$('#tt').tabs('add',{
				title:s,
				content:'<div style="overflow:hidden;width:100%;height:100%;padding:0px;margin:0px;"><iframe scrolling="yes" frameborder="0" src="' + url + '"></iframe></div>',
				closable:true
			});
		}
	}catch(e){}}
};