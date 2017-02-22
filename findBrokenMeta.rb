require 'find'

metafiles = []
Find.find(Dir.getwd()) do |path|
    metafiles << path if path =~ /.*\.meta$/
end

metafiles.each do |fileloc|
    file = File.open(fileloc)
    line = nil
    2.times{line = file.gets}
    puts fileloc if line =~ /.*<<<<<.*/
    file.close
end
puts "Holy rayguns! If there is stuff before me, we have conflicts!"



