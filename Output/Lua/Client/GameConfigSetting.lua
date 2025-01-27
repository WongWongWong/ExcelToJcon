local ConfigVo = {
	data = {	
			{id=1,key="TURN_TIMES",value="10"},
		{id=2,key="TURN_TIMES111"},
		{id=3,key="asd"},
		{id=1,key="Test1",TestValue="hahaha",longTest=123123},
		{id=1,key="Test2",name=12}
	},
	cacheMap = {}
}

export type Type = {
	id:"number",-- 主键
	key:"string",-- 键
	value:"string",-- 值
	TestValue:"string",-- 测试值
	longTest:"number",-- int64
	name:"number",-- 多语言测试
}

function getKey(tableStr,all)
	if not all then
		all = false
	end
	return tableStr.."_"..tostring(all)
end

function ConfigVo:getCache(mapKey)	
	local suc = false
	local vo = nil
	local rw = rawget(ConfigVo.cacheMap, mapKey) or nil
	if rw then
		suc = true
		if typeof(rw) ~= "table" then
			vo = nil
		else
			vo = ConfigVo.cacheMap[mapKey]
		end
	end
	
	return suc,vo
end

function ConfigVo:Get(key,value,all)
	local map = rawset({},key,value)
	return ConfigVo:GetMulti(map,all)
end

function ConfigVo:GetById(value,all)
	return ConfigVo:Get("id",value,all)
end

function ConfigVo:GetMulti(map,all)		
	if not all then
		all = false
	end

	local mapKey = getKey(game.HttpService:JSONEncode(map),all)
	local suc,ret = ConfigVo:getCache(mapKey)
	if suc then
		return ret
	end

	local allTable = {}	
	for i,vo in ipairs(ConfigVo.data) do
		local match = true
		for k,v in pairs(map) do
			if rawget(vo,k) ~= v then
				match = false
			end
		end

		if match then
			table.insert(allTable,vo)
			if not all then
				break
			end
		end
	end

	if all then
		rawset(ConfigVo.cacheMap, mapKey, allTable)
		return allTable		
	end

	if #allTable > 0 then
		local ret = allTable[1]
		rawset(ConfigVo.cacheMap, mapKey, ret)
		return ret
	end

	rawset(ConfigVo.cacheMap, mapKey, -1)
	return nil
end

return ConfigVo
